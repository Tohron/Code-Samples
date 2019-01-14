package daos;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Timestamp;
import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.List;

import beans.Reimbursement;
import database.ConnectionUtil;
import dto.NewReimbursement;
import util.GlobalData;

public class ReimbursementDaoJdbc implements ReimbursementDao {

	@Override
	public List<Reimbursement> getAllReimbursements() {
		ArrayList<Reimbursement> reimbursements = new ArrayList<Reimbursement>();
		try (Connection conn = ConnectionUtil.getConnection()) {
			String query = "SELECT * FROM reimbursement";
			PreparedStatement stmt = conn.prepareStatement(query);
			ResultSet rs = stmt.executeQuery();
			while (rs.next()) {
				Reimbursement r = new Reimbursement(rs.getInt("reimb_id"), rs.getDouble("reimb_amount"), 
						rs.getTimestamp("reimb_submitted"), rs.getTimestamp("reimb_resolved"), rs.getString("reimb_description"), 
						rs.getInt("reimb_author"), rs.getInt("reimb_resolver"), 
						rs.getInt("reimb_status_id"), rs.getInt("reimb_type_id"));
				reimbursements.add(r);
			}
		} catch (SQLException e) {
			e.printStackTrace();
			return null;
		}
		return reimbursements;
	}

	@Override
	public List<Reimbursement> getReimbursementsWithStatus(boolean showPending, boolean showApproved, boolean showRejected) {
		ArrayList<Reimbursement> reimbursements = new ArrayList<Reimbursement>();
		int bool_total = 0;
		if (showPending)
			bool_total++;
		if (showApproved)
			bool_total++;
		if (showRejected)
			bool_total++;
		try (Connection conn = ConnectionUtil.getConnection()) {
			String query;
			PreparedStatement stmt;
			if (showPending || showApproved || showRejected) {
				if (bool_total == 3) {
					query = "SELECT * FROM reimbursement";
					stmt = conn.prepareStatement(query);
				} else {
					if (bool_total == 2) {
						query = "SELECT * FROM reimbursement WHERE reimb_status_id!=?";
						stmt = conn.prepareStatement(query);
						if (!showPending) {
							stmt.setNull(1, java.sql.Types.INTEGER);
						} else if (!showApproved) {
							stmt.setInt(1, GlobalData.reimbursementStatuses.get("APPROVED"));
						} else {
							stmt.setInt(1, GlobalData.reimbursementStatuses.get("REJECTED"));
						}
					} else {
						query = "SELECT * FROM reimbursement WHERE reimb_status_id=?";
						stmt = conn.prepareStatement(query);
						if (showPending) {
							stmt.setNull(1, java.sql.Types.INTEGER);
						} else if (showApproved) {
							stmt.setInt(1, GlobalData.reimbursementStatuses.get("APPROVED"));
						} else {
							stmt.setInt(1, GlobalData.reimbursementStatuses.get("REJECTED"));
						}
					}
				}
			} else {
				return reimbursements;
			}
			ResultSet rs = stmt.executeQuery();
			while (rs.next()) {
				Reimbursement r = new Reimbursement(rs.getInt("reimb_id"), rs.getDouble("reimb_amount"), 
						rs.getTimestamp("reimb_submitted"), rs.getTimestamp("reimb_resolved"), rs.getString("reimb_description"), 
						rs.getInt("reimb_author"), rs.getInt("reimb_resolver"), 
						rs.getInt("reimb_status_id"), rs.getInt("reimb_type_id"));
				reimbursements.add(r);
			}
		} catch (SQLException e) {
			e.printStackTrace();
			return null;
		}
		return reimbursements;
	}

	@Override
	public List<Reimbursement> getUserReimbursements(String username) {
		ArrayList<Reimbursement> reimbursements = new ArrayList<Reimbursement>();
		try (Connection conn = ConnectionUtil.getConnection()) {
			String query = "SELECT * FROM reimbursement WHERE reimb_author = ?";
			PreparedStatement stmt = conn.prepareStatement(query);
			stmt.setInt(1, UsersDao.currentImplementation.getUserId(username));
			ResultSet rs = stmt.executeQuery();
			while (rs.next()) {
				Reimbursement r = new Reimbursement(rs.getInt("reimb_id"), rs.getDouble("reimb_amount"), 
						rs.getTimestamp("reimb_submitted"), rs.getTimestamp("reimb_resolved"), rs.getString("reimb_description"), 
						rs.getInt("reimb_author"), rs.getInt("reimb_resolver"), 
						rs.getInt("reimb_status_id"), rs.getInt("reimb_type_id"));
				reimbursements.add(r);
			}
		} catch (SQLException e) {
			e.printStackTrace();
			return null;
		}
		return reimbursements;
	}

	@Override
	public boolean submitReimbursement(NewReimbursement r, String username) {
		try (Connection conn = ConnectionUtil.getConnection()) {
			String query = "INSERT INTO reimbursement (reimb_amount, reimb_submitted, reimb_description, "
					+ "reimb_author, reimb_type_id) "
					+ "VALUES (?, ?, ?, ?, ?)";
			PreparedStatement stmt = conn.prepareStatement(query);
			stmt.setDouble(1, Double.parseDouble( r.getAmount()));
			stmt.setTimestamp(2, new Timestamp(new Date().getTime()));
			stmt.setString(3, r.getDescription());
			UsersDao ud = UsersDao.currentImplementation;
			stmt.setInt(4, ud.getUserId(username));
			System.out.println("GDR: " + GlobalData.reimbursementTypes);
			System.out.println("Type: " + r.getType());
			stmt.setInt(5, GlobalData.reimbursementTypes.get(r.getType()));
			int result = stmt.executeUpdate();
			return result > 0; // true if row affected, false otherwise
		} catch (SQLException e) {
			e.printStackTrace();
			return false;
		}
	}

	@Override
	public boolean approveReimbursement(int reimbID, int resolverID) {
		try (Connection conn = ConnectionUtil.getConnection()) {
			String query = "UPDATE reimbursement SET reimb_resolved = ?, reimb_resolver = ?, reimb_status_id = ? WHERE reimb_id = ?";
			PreparedStatement stmt = conn.prepareStatement(query);
			stmt.setTimestamp(1, new Timestamp(new java.util.Date().getTime()));
			stmt.setInt(2, resolverID);
			stmt.setInt(3, GlobalData.reimbursementStatuses.get("APPROVED"));
			stmt.setInt(4, reimbID);
			int result = stmt.executeUpdate();
			return result > 0; // true if row affected, false otherwise
		} catch (SQLException e) {
			e.printStackTrace();
			return false;
		}
	}

	@Override
	public boolean denyReimbursement(int reimbID, int resolverID) {
		try (Connection conn = ConnectionUtil.getConnection()) {
			String query = "UPDATE reimbursement SET reimb_resolved = ?, reimb_resolver = ?, reimb_status_id = ? WHERE reimb_id = ?";
			PreparedStatement stmt = conn.prepareStatement(query);
			stmt.setTimestamp(1, new Timestamp(new java.util.Date().getTime()));
			stmt.setInt(2, resolverID);
			stmt.setInt(3, GlobalData.reimbursementStatuses.get("REJECTED"));
			stmt.setInt(4, reimbID);
			int result = stmt.executeUpdate();
			return result > 0; // true if row affected, false otherwise
		} catch (SQLException e) {
			e.printStackTrace();
			return false;
		}
	}

}
