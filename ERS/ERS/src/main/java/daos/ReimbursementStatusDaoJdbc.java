package daos;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.HashMap;

import database.ConnectionUtil;

public class ReimbursementStatusDaoJdbc implements ReimbursementStatusDao {

	public HashMap<String, Integer> getAllReimbursementStatuses() {
		try (Connection conn = ConnectionUtil.getConnection()) {
			String query = "SELECT * FROM reimbursement_status";
			PreparedStatement stmt = conn.prepareStatement(query);
			ResultSet rs = stmt.executeQuery();
			HashMap<String, Integer> map = new HashMap<String, Integer>();
			while (rs.next()) {
				map.put(rs.getString("reimb_status"), rs.getInt("reimb_status_id"));
			}
			return map;
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		return null;
	}

	public String getStatus(int statusId) {
		try (Connection conn = ConnectionUtil.getConnection()) {
			String query = "SELECT * FROM reimbursement_status WHERE reimb_status_id=?";
			PreparedStatement stmt = conn.prepareStatement(query);
			stmt.setInt(1, statusId);
			ResultSet rs = stmt.executeQuery();
			if (rs.next()) {
				return rs.getString("reimb_status");
			}
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return null;
	}
}
