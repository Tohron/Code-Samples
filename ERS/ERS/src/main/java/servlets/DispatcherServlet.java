package servlets;

import java.io.IOException;
import java.util.HashMap;

import javax.servlet.ServletConfig;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import util.GlobalData;
import util.ResponseMapper;

/**
 * Responds to request to show past reimbursements,
 * receives new reimbursement requests, and updates
 * the client with changes to past reimbursements.
 * @author Aaron
 *
 */
public class DispatcherServlet extends HttpServlet {
	// Identify the logged-in username using the session ID as a key
	private HashMap<String, String> loggedEmployees;
	private HashMap<String, String> loggedFManagers;
	
	private LoginController lc = new LoginController();
	private EmployeeController ec = new EmployeeController();
	private FinanceController fc = new FinanceController();
	
	@Override
	public void init(ServletConfig config) throws ServletException {
		System.out.println("initializing servlet");

		String configParam = config.getInitParameter("specifParam");
		System.out.println("config param: " + configParam);

		String contextParam = config.getServletContext().getInitParameter("sharedParam");
		System.out.println("context param: " + contextParam);
		
		loggedEmployees = new  HashMap<String, String>();
		loggedFManagers = new  HashMap<String, String>();
		GlobalData.getImplementation();
	}
	
	@Override
	protected void service(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
		resp.addHeader("Access-Control-Allow-Origin", "http://127.0.0.1:5500");
		resp.addHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, DELETE, HEAD");
		resp.addHeader("Access-Control-Allow-Headers",
				"Origin, Methods, Credentials, X-Requested-With, Content-Type, Accept");
		resp.addHeader("Access-Control-Allow-Credentials", "true");
		resp.setContentType("application/json");
		String session = req.getSession().getId();
		
		
		String uri = req.getRequestURI();
		String context = "ERS";
		uri = uri.substring(context.length() + 2, uri.length());
		//log.debug("request made with uri: " + uri);
		System.out.println("Received request with uri:" + uri);
		if (uri.startsWith("employee")) {
			ec.process(req, resp, loggedEmployees.get(session));
		} else if (uri.startsWith("finance")) {
			fc.process(req, resp, loggedFManagers.get(session));
		} else if (uri.startsWith("login")) {
			lc.process(req, resp, loggedEmployees, loggedFManagers);
		} else if (uri.startsWith("e_logout")) {
			loggedEmployees.remove(session);
		} else if (uri.startsWith("f_logout")) {
			loggedFManagers.remove(session);
		} else {
			resp.setStatus(404);
		}
		
	}
	// HashMap passed to controllers
	/*
	public String getUser(String sessionID) {
		return loggedUsers.get(sessionID);
	}
	public void setUser(String sessionID, String userID) {
		loggedUsers.put(sessionID, userID);
	}*/
}
