package daos;

import java.util.List;

import beans.Reimbursement;
import dto.NewReimbursement;

public interface ReimbursementDao {
	public ReimbursementDao currentImplementation = new ReimbursementDaoJdbc();
	
	public List<Reimbursement> getAllReimbursements();
	
	public List<Reimbursement> getReimbursementsWithStatus(boolean showPending, boolean showApproved, boolean showRejected);
	
	public List<Reimbursement> getUserReimbursements(String username);
	
	/**
	 * Submits a reimbursement with the given info to the database
	 * @param r the Reimbursement containing the info that will be submitted
	 * 			to the database as a reimbursement
	 * @return true if database operation completed, false otherwise
	 */
	public boolean submitReimbursement(NewReimbursement r, String username);
	
	/**
	 * Approves the reimbursement with the given ID.
	 * @param reimbID the ID of the reimbursement to approve
	 * @return true if database operation completed, false otherwise
	 */
	public boolean approveReimbursement(int reimbID, int resolverID);
	/**
	 * Denies the reimbursement with the given ID.
	 * @param reimbID the ID of the reimbursement to deny
	 * @return true if database operation completed, false otherwise
	 */
	public boolean denyReimbursement(int reimbID, int resolverID);
}
