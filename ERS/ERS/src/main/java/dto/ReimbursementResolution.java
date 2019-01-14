package dto;

import java.sql.Timestamp;

public class ReimbursementResolution {
	private int id;
	private int resolver;
	//private long resolved; // resolved is calculated from submission arrival
	private String status;
	public ReimbursementResolution(int id, int resolver, String status) {
		super();
		this.id = id;
		this.resolver = resolver;
		this.status = status;
	}

	public int getId() {
		return id;
	}
	public int getResolver() {
		return resolver;
	}
	public String getStatus() {
		return status;
	}
}
