package makarosoft.VmsWeb;

import java.util.Map;
import java.util.HashMap;
import java.io.BufferedReader;
import java.io.IOException;
import java.util.StringTokenizer;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

public class Request {
	private String _method;
	private String _path;
	private String _fullUrl;
	private Map<String, String> _headers = new HashMap<String, String>();
	private Map<String, String> _queryParameters = new HashMap<String, String>();
	private BufferedReader _in;
	Logger _logger = LogManager.getLogger(Request.class);

	public Request(BufferedReader in) {
		_in = in;
	}

	public String getMethod() {
		return _method;
	}
	
	public String getBody() throws IOException {
		String lengthString = getHeader("Content-Length");
		if (lengthString == null || lengthString.trim().equals("")) return "";
		
		int length = Integer.parseInt(lengthString.trim());
		_logger.debug("Length = {}", length);
		if (length == 0) return "";
		
		StringBuffer sb = new StringBuffer(length);
		char[] theChars = new char[8000];
		
		int charsRead = _in.read(theChars, 0, theChars.length);
		while (charsRead != -1) {			
			sb.append(theChars, 0, charsRead);

			if (sb.length() == length) {
				_logger.debug("");
				_logger.debug("Length matches {}", length);
				return sb.toString();
			}
			charsRead = _in.read(theChars, 0, theChars.length);
		}
		// TODO I think I probably need to check to make sure it matches the length
		_logger.debug("expecting {} but got {}", length, sb.length());
		return sb.toString();
	}

	public String getPath() {
		return _path;
	}

	public String getFullUrl() {
		return _fullUrl;
	}

	// TODO support mutli-value headers
	public String getHeader(String headerName) {
		return _headers.get(headerName);
	}

	public String getParameter(String paramName) {
		return _queryParameters.get(paramName);
	}

	private void parseQueryParameters(String queryString) {
		for (String parameter : queryString.split("&")) {
			int separator = parameter.indexOf('=');
			if (separator > -1) {
				_queryParameters.put(parameter.substring(0, separator), parameter.substring(separator + 1));
			} else {
				_queryParameters.put(parameter, null);
			}
		}
	}

	public boolean parse() throws IOException {
		String initialLine = _in.readLine();
		if (initialLine == null) {
			_logger.debug("*************: {}", initialLine);
			return false;
		}
		_logger.debug(initialLine);
		StringTokenizer tok = new StringTokenizer(initialLine);
		String[] components = new String[3];
		for (int i = 0; i < components.length; i++) {
			// TODO support HTTP/1.0?
			if (tok.hasMoreTokens()) {
				components[i] = tok.nextToken();
			} else {
				return false;
			}
		}

		_method = components[0];
		_fullUrl = components[1];

		// Consume headers
		while (true) {
			String headerLine = _in.readLine();
			_logger.debug(headerLine);
			if (headerLine.length() == 0) {
				break;
			}

			int separator = headerLine.indexOf(":");
			if (separator == -1) {
				return false;
			}
			_headers.put(headerLine.substring(0, separator), headerLine.substring(separator + 1));
		}

		// TODO should look for host header, Connection: Keep-Alive header,
		// Content-Transfer-Encoding: chunked

		if (components[1].indexOf("?") == -1) {
			_path = components[1];
		} else {
			_path = components[1].substring(0, components[1].indexOf("?"));
			parseQueryParameters(components[1].substring(components[1].indexOf("?") + 1));
		}

		if ("/".equals(_path)) {
			_path = "/index.html";
		}

		return true;
	}

	public String toString() {
		return _method + " " + _path + " " + _headers.toString();
	}
}
