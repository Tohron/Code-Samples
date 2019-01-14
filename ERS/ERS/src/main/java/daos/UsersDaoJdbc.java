package daos;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.HashMap;

import beans.User;
import database.ConnectionUtil;

public class UsersDaoJdbc implements UsersDao {

	public String getFirstName(int userId) {
		// TODO Auto-generated method stub
		return null;
	}

	public String getLastName(int userId) {
		// TODO Auto-generated method stub
		return null;
	}

	public String getEmail(int userId) {
		// TODO Auto-generated method stub
		return null;
	}

	public int getRole(int userID) {
		// TODO Auto-generated method stub
		return 0;
	}

	public boolean verifyLogin(String username, String password, int role_id) {
		String query = "SELECT ers_password, user_role_id FROM users WHERE ers_username = ?";
		try (Connection conn = ConnectionUtil.getConnection()) {
			PreparedStatement stmt = conn.prepareStatement(query);
			stmt.setString(1, username);
			ResultSet rs = stmt.executeQuery();
			if (rs.next()) {
				if (password.equals(rs.getString("ers_password")) && role_id == rs.getInt("user_role_id")) {
					return true;
				}
			}
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return false;
	}

	public HashMap<Integer, User> getCurrentUsers() {
		HashMap<Integer, User> curUsers = new HashMap<Integer, User>();
		String query = "SELECT * FROM users";
		try (Connection conn = ConnectionUtil.getConnection()) {
			PreparedStatement stmt = conn.prepareStatement(query);
			ResultSet rs = stmt.executeQuery();
			while (rs.next()) {
				User u = new User(rs.getInt("ers_users_id"),
							rs.getString("ers_username"),
							rs.getString("ers_password"),
							rs.getString("user_first_name"),
							rs.getString("user_last_name"),
							rs.getString("user_email"),
							rs.getInt("user_role_id"));
				curUsers.put(rs.getInt("ers_users_id"), u);
			}
			return curUsers;
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return null;
	}

	@Override
	public int getUserId(String username) {
		String query = "SELECT ers_users_id FROM users WHERE ers_username=?";
		try (Connection conn = ConnectionUtil.getConnection()) {
			PreparedStatement stmt = conn.prepareStatement(query);
			stmt.setString(1, username);
			ResultSet rs = stmt.executeQuery();
			if (rs.next()) {
				return rs.getInt("ers_users_id");
			}
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return 0;
	}

}
