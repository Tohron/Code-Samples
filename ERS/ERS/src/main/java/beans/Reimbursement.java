package beans;

import java.sql.Timestamp;
import java.text.SimpleDateFormat;
import java.util.Date;

import daos.ReimbursementStatusDao;
import daos.ReimbursementTypeDao;
import util.GlobalData;

public class Reimbursement {
	private int id;
	private double amount;
	private Timestamp submitted;
	private Timestamp resolved;
	private String submittedString;
	private String resolvedString;
	private String description;
	private int authorID;
	private int resolverID;
	private int statusID;
	private int typeID;
	private String author;
	private String resolver;
	private String status;
	private String type;
	
	private SimpleDateFormat sdf = new SimpleDateFormat("M/dd/yyyy h:mm a");
	
	public Reimbursement(int id, double amount, Timestamp submitted, Timestamp resolved, String description, 
			int authorID, int resolverID, int statusID, int typeID) {
		this.id = id;
		this.amount = amount;
		this.submitted = submitted;
		this.resolved = resolved;
		this.description = description;
		this.authorID = authorID;
		this.resolverID = resolverID;
		this.statusID = statusID;
		this.typeID = typeID;
		author = GlobalData.currentUsers.get(authorID).getFirstName() +" "+ GlobalData.currentUsers.get(authorID).getLastName();
		//System.out.println("Resolver ID: " + resolverID); // is 0 for null
		if (resolverID > 0) {
			resolver = GlobalData.currentUsers.get(resolverID).getFirstName() +" "+ GlobalData.currentUsers.get(resolverID).getLastName();
		} else {
			resolver = "";
		}
		if (statusID > 0) {
			status = ReimbursementStatusDao.currentImplementation.getStatus(statusID);
		} else {
			status = "PENDING";
		}
		type = GlobalData.reimbursementRevTypes.get(typeID);
		submittedString = sdf.format(new Date(submitted.getTime()));
		if (resolved != null) {
			resolvedString = sdf.format(new Date(resolved.getTime()));
		} else {
			resolvedString = "";
		}
	}

	public int getId() {
		return id;
	}

	public double getAmount() {
		return amount;
	}

	public Timestamp getSubmitted() {
		return submitted;
	}

	public Timestamp getResolved() {
		return resolved;
	}

	public String getSubmittedString() {
		return submittedString;
	}

	public String getResolvedString() {
		return resolvedString;
	}

	public String getDescription() {
		return description;
	}

	public int getAuthorID() {
		return authorID;
	}

	public int getResolverID() {
		return resolverID;
	}

	public int getStatusID() {
		return statusID;
	}

	public int getTypeID() {
		return typeID;
	}

	public String getAuthor() {
		return author;
	}

	public String getResolver() {
		return resolver;
	}

	public String getStatus() {
		return status;
	}

	public String getType() {
		return type;
	}

	public SimpleDateFormat getSdf() {
		return sdf;
	}

	
}
