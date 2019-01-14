package util;

import java.io.IOException;

import javax.servlet.http.HttpServletResponse;

import com.fasterxml.jackson.databind.ObjectMapper;

public class ResponseMapper {
	private static ObjectMapper om = new ObjectMapper();

	public static void convertAndAttach(Object o, HttpServletResponse resp) throws IOException {
		String json = om.writeValueAsString(o);
		resp.getWriter().write(json);
	}
}
