package makarosoft.VmsWeb;

import java.util.Map;
import java.util.HashMap;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;
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
	private JwtVerifier.VerifiedToken _verifiedToken;

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

	public void setVerifiedToken(JwtVerifier.VerifiedToken token) {
		_verifiedToken = token;
	}
	public JwtVerifier.VerifiedToken getVerifiedToken() {
		return _verifiedToken;
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
				String key = parameter.substring(0, separator);
				String value = parameter.substring(separator + 1);
				try {
					_queryParameters.put(URLDecoder.decode(key, "UTF-8"), URLDecoder.decode(value,"UTF-8"));
				} catch (UnsupportedEncodingException e) {
					_logger.error("Unable to decode: key = {}, value = {}", key, value);
					_queryParameters.put(key, value);
				}
			} else {
				_queryParameters.put(parameter, null);
			}
		}
	}

	public boolean parse() throws IOException {
		String initialLine = _in.readLine();
		if (initialLine == null) {
			_logger.debug("*************: nothing to parse");
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
			_logger.trace(headerLine);
			if (headerLine.length() == 0) {
				break;
			}

			int separator = headerLine.indexOf(":");
			if (separator == -1) {
				return false;
			}
			
			// get rid of leading space, if any
			int add = 0;
			//String shit = headerLine.substring(separator + 1, separator + 1);
			if (headerLine.substring(separator + 1, separator + 2).equals(" ")) add++;
			_headers.put(headerLine.substring(0, separator), headerLine.substring(separator + 1 + add));
		}

		// TODO should look for host header, Connection: Keep-Alive header,
		// Content-Transfer-Encoding: chunked

		if (components[1].indexOf("?") == -1) {
			String raw = components[1];
			_path = URLDecoder.decode(raw, "UTF-8");
		} else {
			String raw = components[1].substring(0, components[1].indexOf("?"));
			_path = URLDecoder.decode(raw, "UTF-8");			
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
