package daos;

import java.util.HashMap;

public interface ReimbursementTypeDao {
	ReimbursementTypeDao currentImplementation = new ReimbursementTypeDaoJdbc();
	/**
	 * Retrieves all reimbursement types from the database.
	 * @return A way to get a given type using its ID.
	 */
	public HashMap<String, Integer> getAllReimbursementTypes();
	public HashMap<Integer, String> getAllRevReimbursementTypes();
	
	//public String getType(int typeId);
}
