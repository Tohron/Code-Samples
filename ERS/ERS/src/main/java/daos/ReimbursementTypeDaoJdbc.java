package daos;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.HashMap;

import database.ConnectionUtil;

public class ReimbursementTypeDaoJdbc implements ReimbursementTypeDao {

	public HashMap<String, Integer> getAllReimbursementTypes() {
		try (Connection conn = ConnectionUtil.getConnection()) {
			String query = "SELECT * FROM reimbursement_type";
			PreparedStatement stmt = conn.prepareStatement(query);
			ResultSet rs = stmt.executeQuery();
			HashMap<String, Integer> map = new HashMap<String, Integer>();
			while (rs.next()) {
				map.put(rs.getString("reimb_type"), rs.getInt("reimb_type_id"));
			}
			return map;
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		return null;
	}
	public HashMap<Integer, String> getAllRevReimbursementTypes() {
		try (Connection conn = ConnectionUtil.getConnection()) {
			String query = "SELECT * FROM reimbursement_type";
			PreparedStatement stmt = conn.prepareStatement(query);
			ResultSet rs = stmt.executeQuery();
			HashMap<Integer, String> map = new HashMap<Integer, String>();
			while (rs.next()) {
				map.put(rs.getInt("reimb_type_id"), rs.getString("reimb_type"));
			}
			return map;
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		return null;
	}
	/*
	@Override
	public String getType(int typeId) {
		try (Connection conn = ConnectionUtil.getConnection()) {
			String query = "SELECT * FROM reimbursement_type WHERE reimb_type_id=?";
			PreparedStatement stmt = conn.prepareStatement(query);
			stmt.setInt(1, typeId);
			ResultSet rs = stmt.executeQuery();
			if (rs.next()) {
				return rs.getString("reimb_type");
			}
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return null;
	}
	*/
}
