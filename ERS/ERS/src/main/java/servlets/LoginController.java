package servlets;

import java.io.IOException;
import java.util.Arrays;
import java.util.HashMap;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.log4j.Logger;

import com.fasterxml.jackson.core.JsonParseException;
import com.fasterxml.jackson.databind.JsonMappingException;
import com.fasterxml.jackson.databind.ObjectMapper;

import daos.UserRolesDao;
import daos.UsersDao;
import dto.Credential;
import util.GlobalData;

public class LoginController {
	private Logger log = Logger.getRootLogger();
	private UsersDao ud = UsersDao.currentImplementation;
	private ObjectMapper om = new ObjectMapper();
	
	void process(HttpServletRequest req, HttpServletResponse resp, 
			HashMap<String, String> loggedEmployees, HashMap<String, String> loggedFManagers) throws IOException {
		String method = req.getMethod();
		System.out.println("Request Method: " + method);
		switch (method) {
		case "GET":
			//processGet(req, resp);
			break;
		case "POST":
			processPost(req, resp, loggedEmployees, loggedFManagers);
			break;
		case "OPTIONS":
			return;
		default:
			resp.setStatus(404);
			break;
		}
	}
	
	private void processPost(HttpServletRequest req, HttpServletResponse resp, 
			HashMap<String, String> loggedEmployees, HashMap<String, String> loggedFManagers) throws JsonParseException, JsonMappingException, IOException {
		String uri = req.getRequestURI();
		String context = "ERS";
		uri = uri.substring(context.length() + 2, uri.length());
		if ("login".equals(uri)) {
			log.info("attempting to log in");
			Credential cred = om.readValue(req.getReader(), Credential.class);
			System.out.println("Role: " + cred.getRole());
			int roleID = GlobalData.userRoles.get(cred.getRole());
			String session = req.getSession().getId();
			System.out.println("Session: " + session);
			if(!ud.verifyLogin(cred.getUsername(), cred.getPassword(), roleID)) {
				resp.setStatus(403);
			} else if ("FINANCE".equals(cred.getRole())){
				loggedFManagers.put(session, cred.getUsername());
				System.out.println("Logged in as " + cred.getUsername());
			} else if ("EMPLOYEE".equals(cred.getRole())){
				loggedEmployees.put(session, cred.getUsername());
				System.out.println("Logged in as " + cred.getUsername());
			}
		} else {
			resp.setStatus(404);
			return;
		}
	}
}
