package util;

import java.util.HashMap;

import beans.User;
import daos.ReimbursementStatusDao;
import daos.ReimbursementTypeDao;
import daos.UserRolesDao;
import daos.UsersDao;

public class GlobalData {
	private static GlobalData currentImplementation = null; 
	
	public static final HashMap<String, Integer> reimbursementStatuses 
			= ReimbursementStatusDao.currentImplementation.getAllReimbursementStatuses();
	public static final HashMap<String, Integer> reimbursementTypes 
			= ReimbursementTypeDao.currentImplementation.getAllReimbursementTypes();
	public static final HashMap<Integer, String> reimbursementRevTypes 
		= ReimbursementTypeDao.currentImplementation.getAllRevReimbursementTypes();
	public static final HashMap<String, Integer> userRoles 
			= UserRolesDao.currentImplementation.getAllUserRoles();
	
	public static HashMap<Integer, User> currentUsers = UsersDao.currentImplementation.getCurrentUsers();
	
	private GlobalData() {
		
	}
	
	public static GlobalData getImplementation() {
		if (currentImplementation == null) {
			currentImplementation = new GlobalData();
		}
		return currentImplementation;
	}
}
