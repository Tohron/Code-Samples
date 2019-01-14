package daos;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.HashMap;

import database.ConnectionUtil;
import util.GlobalData;

public class UserRolesDaoJdbc implements UserRolesDao {
	
	public HashMap<String, Integer> getAllUserRoles() {
		try (Connection conn = ConnectionUtil.getConnection()) {
			String query = "SELECT * FROM user_roles";
			PreparedStatement stmt = conn.prepareStatement(query);
			ResultSet rs = stmt.executeQuery();
			HashMap<String, Integer> map = new HashMap<String, Integer>();
			while (rs.next()) {
				map.put(rs.getString("user_role"), rs.getInt("ers_user_role_id"));
			}
			return map;
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		return null;
	}

}
