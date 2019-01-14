package daos;

import java.util.HashMap;

public interface ReimbursementStatusDao {
	ReimbursementStatusDao currentImplementation = new ReimbursementStatusDaoJdbc();
	/**
	 * Retrieves all reimbursement statuses from the database.
	 * @return A way to get a given status using its ID.
	 */
	public HashMap<String, Integer> getAllReimbursementStatuses();
	
	public String getStatus(int statusId);
}
