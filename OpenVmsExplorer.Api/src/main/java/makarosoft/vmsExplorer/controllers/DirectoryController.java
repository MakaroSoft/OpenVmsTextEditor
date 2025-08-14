package makarosoft.vmsExplorer.controllers;

import java.io.IOException;
import java.util.List;

import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

import com.google.gson.Gson;

import makarosoft.VmsWeb.ApiController;
import makarosoft.VmsWeb.Request;
import makarosoft.VmsWeb.Response;
import makarosoft.vmsExplorer.Directory.DirectoryFactory;
import makarosoft.vmsExplorer.Directory.IDirectory;
import makarosoft.vmsExplorer.util.Authz;

public class DirectoryController extends ApiController {
	
	Logger _logger = LogManager.getLogger(DirectoryController.class);
	
	@Override
	public void get(Request request, Response response) throws IOException {
		try {
			String folder = getRelativePath(request);
			if (folder == null) {
				response.setResponseCode(404, "Not Found");
				return;
			}
			
            String include = request.getParameter("include");
			String exclude = request.getParameter("exclude");
			String showHistoryString = request.getParameter("showHistory");
			boolean showHistory = Boolean.parseBoolean(showHistoryString);
			
            makarosoft.VmsWeb.JwtVerifier.VerifiedToken token = request.getVerifiedToken();
            java.util.List<String> allowed = null;
            java.util.Set<String> allowedDisks = null;
            boolean enforce = Authz.isUserOnlyRole(token);
            String normFolder = normalizeToPath(folder);
            if (enforce) {
                allowed = Authz.getAllowedFolders(token);
                allowedDisks = Authz.allowedDisks(allowed);
                // ensure requested disk is allowed
                String disk = getDiskLower(normFolder);
                if (disk != null && !allowedDisks.contains(disk)) {
                    response.setResponseCode(403, "Forbidden");
                    return;
                }
                boolean isRoot = isDiskRoot(normFolder);
                if (!isRoot && !(isWithinAllowed(normFolder, allowed) || isAncestorOfAnyRule(normFolder, allowed))) {
                    response.setResponseCode(403, "Forbidden");
                    return;
                }
            }

            List<makarosoft.vmsExplorer.models.File> folderContents = getFolderContent(folder, include, exclude, showHistory);
            if (folderContents == null) {
                if (enforce) {
                    boolean isRoot = isDiskRoot(normFolder);
                    if (isRoot || isWithinAllowed(normFolder, allowed) || isAncestorOfAnyRule(normFolder, allowed)) {
                        folderContents = new java.util.ArrayList<>();
                    } else {
                        response.setResponseCode(404, "Not Found");
                        return;
                    }
                } else {
                    response.setResponseCode(404, "Not Found");
                    return;
                }
            }

            if (enforce) {
                // Filter results to only those children that are allowed or lead to an allowed subtree
                java.util.ArrayList<makarosoft.vmsExplorer.models.File> filtered = new java.util.ArrayList<>();
                String base = normFolder;
                for (makarosoft.vmsExplorer.models.File f : folderContents) {
                    boolean isDir = f.Dir != null && f.Dir.booleanValue();
                    String childPath = base.endsWith("/") ? base + f.Name.toLowerCase() : base + "/" + f.Name.toLowerCase();
                    if (isDiskRoot(base)) {
                        if (isAllowedChildAtDiskRoot(childPath, allowed)) filtered.add(f);
                    } else {
                        if (!isDir) {
                            if (isWithinAllowed(childPath, allowed) || isExactAllowedDir(base, allowed)) filtered.add(f);
                        } else {
                            // inside an allowed subtree, include directories that are within or ancestors of any rule
                            if (isWithinAllowed(childPath, allowed) || isAncestorOfAnyRule(childPath, allowed)) filtered.add(f);
                        }
                    }
                }
                folderContents = filtered;
            }

			Gson gson = new Gson();
			String json = gson.toJson(folderContents);
			
			response.setResponseCode(200, "OK");
			response.addHeader("Content-Type", "application/json");
			response.addBody(json);
		} catch (Exception e) {
			_logger.error(e.getMessage(), e);
			response.setResponseCode(500, "Internal Server Error");
		}
	}
	
    private List<makarosoft.vmsExplorer.models.File> getFolderContent(String folder, String include, String exclude, boolean showHistory) {
		IDirectory directory = new DirectoryFactory().Create(folder);
		return directory.getFiles(include, exclude,showHistory);		
	}

    private String normalizeToPath(String text) {
        if (text == null) return "";
        String t = text.trim().toLowerCase();
        if (t.endsWith("/") && !t.endsWith(":/")) t = t.substring(0, t.length()-1);
        if (!t.contains(":/")) {
            if (t.endsWith(":")) t = t + "/"; else t = t + ":/";
        }
        return t;
    }

    private String getDiskLower(String text) {
        if (text == null) return null;
        String t = text.trim().toLowerCase();
        int idx = t.indexOf(":");
        if (idx > 0) return t.substring(0, idx + 1);
        return null;
    }

    private boolean isDiskRoot(String norm) {
        if (norm == null) return false;
        int idx = norm.indexOf(":");
        return idx > 0 && norm.equals(norm.substring(0, idx + 1) + "/");
    }

    private boolean isWithinAllowed(String normPath, java.util.List<String> allowed) {
        if (allowed == null) return false;
        for (String rule : allowed) {
            String r = normalizeToPath(rule);
            boolean wildcard = r.endsWith("/*");
            String base = wildcard ? r.substring(0, r.length()-2) : r;
            if (wildcard) {
                if (normPath.equals(base) || normPath.startsWith(base + "/")) return true;
            } else {
                if (normPath.equals(base)) return true;
            }
        }
        return false;
    }

    private boolean isAncestorOfAnyRule(String childPath, java.util.List<String> allowed) {
        if (allowed == null) return false;
        for (String rule : allowed) {
            String r = normalizeToPath(rule);
            boolean wildcard = r.endsWith("/*");
            String base = wildcard ? r.substring(0, r.length()-2) : r;
            if (base.startsWith(childPath + "/")) return true;
        }
        return false;
    }

    private boolean isAllowedChildAtDiskRoot(String childPath, java.util.List<String> allowed) {
        // include child directory if it's the base of any allowed rule, or an ancestor of any allowed rule
        return isWithinAllowed(childPath, allowed) || isAncestorOfAnyRule(childPath, allowed);
    }

    private boolean isExactAllowedDir(String normDir, java.util.List<String> allowed) {
        if (allowed == null) return false;
        String dir = normalizeToPath(normDir);
        for (String rule : allowed) {
            String r = normalizeToPath(rule);
            if (r.endsWith("/*")) continue; // wildcard means subfolders too, handled elsewhere
            if (r.equals(dir)) return true; // exact directory rule: files inside are allowed
        }
        return false;
    }
}
