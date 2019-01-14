package servlets;

import java.io.IOException;
import java.util.Arrays;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.log4j.Logger;

import com.fasterxml.jackson.core.JsonParseException;
import com.fasterxml.jackson.databind.JsonMappingException;
import com.fasterxml.jackson.databind.ObjectMapper;

import util.ResponseMapper;

import beans.Reimbursement;
import daos.ReimbursementDao;
import daos.UsersDao;
import dto.Credential;
import dto.NewReimbursement;

public class EmployeeController {
	private Logger log = Logger.getRootLogger();
	private ReimbursementDao rd = ReimbursementDao.currentImplementation;
	private ObjectMapper om = new ObjectMapper();
	
	
	void process(HttpServletRequest req, HttpServletResponse resp, String username) throws IOException {
		String method = req.getMethod();
		log.trace("reqiest made to champion controller with method: " + req.getMethod());
		switch (method) {
		case "GET":
			processGet(req, resp, username);
			break;
		case "POST":
			processPost(req, resp, username);
			break;
		case "OPTIONS":
			return;
		default:
			resp.setStatus(404);
			break;
		}
	}
	
	/**
	 * Get can receive requests for all the reimbursements a given employee has submitted
	 * @param req
	 * @param resp Sends back List of reimbursements as a JSON
	 * @throws IOException
	 */
	private void processGet(HttpServletRequest req, HttpServletResponse resp, String username) throws IOException {
		String uri = req.getRequestURI();
		String context = "ERS";
		uri = uri.substring(context.length() + 2, uri.length());
		String[] uriArray = uri.split("/");
		System.out.println(Arrays.toString(uriArray));
		if (uriArray.length == 1) {
			//int userId = Integer.parseInt(uriArray[1]); // Set from GET parameters
			System.out.println("Getting Reimbursements For: " + username);
			List<Reimbursement> userReimbursements = ReimbursementDao.currentImplementation.getUserReimbursements(username);
			if (userReimbursements.size() > 0) {
				//System.out.println("Type: " + userReimbursements.get(0).getType());
				//System.out.println("Status: " + userReimbursements.get(0).getStatus());
				System.out.println("Submitted: " + userReimbursements.get(0).getSubmittedString());
			}
			ResponseMapper.convertAndAttach(userReimbursements, resp);
			return;
		} else if (uriArray.length == 2) {
			ResponseMapper.convertAndAttach(username, resp);
		} else {
			resp.setStatus(404);
			return;
		}
	}
	
	/**
	 * Post can receive requests to store a newly created reimbursement request
	 * @param req Includes the reimbursement object 
	 * @param resp
	 * @throws JsonParseException
	 * @throws JsonMappingException
	 * @throws IOException
	 */
	private void processPost(HttpServletRequest req, HttpServletResponse resp, String username)
			throws JsonParseException, JsonMappingException, IOException {
		String uri = req.getRequestURI();
		String context = "ERS";
		uri = uri.substring(context.length() + 2, uri.length());
		if ("employee/add_reimb".equals(uri)) {
			NewReimbursement nr = om.readValue(req.getReader(), NewReimbursement.class);
			//Reimbursement r = om.readValue(req.getReader(), Reimbursement.class);
			boolean success = rd.submitReimbursement(nr, username);
			// --------------- Need to set resp if success == false
		} else {
			resp.setStatus(404);
			return;
		}
	}
}
