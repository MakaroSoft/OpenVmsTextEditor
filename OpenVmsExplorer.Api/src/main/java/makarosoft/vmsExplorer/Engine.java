package makarosoft.vmsExplorer;
import java.io.IOException;
import java.nio.file.Paths;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import makarosoft.VmsWeb.HttpServer;
import makarosoft.VmsWeb.JwtVerifier;
import makarosoft.vmsExplorer.controllers.DirectoryController;
import makarosoft.vmsExplorer.controllers.DiskController;
import makarosoft.vmsExplorer.controllers.FileController;

public class Engine {

	public static void main(String[] args) throws IOException {
		Logger logger = LogManager.getLogger(Engine.class);
		logger.info("Starting makarosoft.vmsExplorer.Engine");
		try {
		int port = 0;
		String publicKeyPemPath = null;
		String issuer = null;
		String audience = null;
		
		for (int index = 0; index < args.length; index++) {
			String arg = args[index];
			if (arg.equalsIgnoreCase("-port")) {
				index++;
				port = Integer.parseInt(args[index]);
				continue;
			}
			if (arg.equalsIgnoreCase("-publicKeyPemPath")) {
				index++;
				publicKeyPemPath = args[index];
				continue;
			}
			if (arg.equalsIgnoreCase("-issuer")) {
				index++;
				issuer = args[index];
				continue;
			}
			if (arg.equalsIgnoreCase("-audience")) {
				index++;
				audience = args[index];
				continue;
			}
		}
		
		if (port == 0) {
			logger.error("port was not set up. Ex: -port 8001");
			return;
		}
		logger.debug("port number is {}", port);
		
		if (publicKeyPemPath == null || publicKeyPemPath.trim().equals("")) {
			logger.error("publicKeyPemPath was not set up. Ex: -publicKeyPemPath /somewhere/jwt-public.pem");
			return;			
		}
		logger.debug("publicKeyPemPath is {}", publicKeyPemPath);
		
		if (issuer == null || issuer.trim().equals("")) {
			logger.error("issuer was not set up. Ex: -issuer https://your-dotnet-site");
			return;			
		}
		logger.debug("issuer is {}", issuer);

		if (audience == null || audience.trim().equals("")) {
			logger.error("audience was not set up. Ex: -audience editor-api");
			return;			
		}		
		logger.debug("audience is {}", audience);
		
		JwtVerifier jwtVerifier = new JwtVerifier(Paths.get(publicKeyPemPath), issuer, audience, 60);
		
		
		HttpServer server = new HttpServer(port, jwtVerifier);
		server.addController("/api/disk", new DiskController());
		server.addController("/api/directory", new DirectoryController());
		server.addController("/api/File", new FileController());
		server.start();
		} catch (Exception e) {
			logger.error(e.getMessage(), e);
		}
	}
	
}
