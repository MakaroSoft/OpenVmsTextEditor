package makarosoft.VmsWeb;

import java.io.IOException;
import java.io.OutputStream;
import java.util.Map;
import java.util.HashMap;

/**
 * Encapsulate an HTTP Response. Mostly just wrap an output stream and provide
 * some state.
 */
public class Response {
	private OutputStream _out;
	private int _statusCode;
	private String _statusMessage;
	private Map<String, String> _headers = new HashMap<String, String>();
	private String _body;

	public Response(OutputStream out) {
		_out = out;
	}

	public void setResponseCode(int statusCode, String statusMessage) {
		_statusCode = statusCode;
		_statusMessage = statusMessage;
	}

	public void addHeader(String headerName, String headerValue) {
		_headers.put(headerName, headerValue);
	}

	public void addBody(String body) {
		_headers.put("Content-Length", Integer.toString(body.length()));
		_body = body;
	}

	public void send() throws IOException {
		_headers.put("Connection", "Close");
		_out.write(("HTTP/1.1 " + _statusCode + " " + _statusMessage + "\r\n").getBytes());
		for (String headerName : _headers.keySet()) {
			_out.write((headerName + ": " + _headers.get(headerName) + "\r\n").getBytes());
		}
		_out.write("\r\n".getBytes());
		if (_body != null) {
			_out.write(_body.getBytes());
		}
	}
}
