package makarosoft.vmsExplorer.util;

import java.text.ParseException;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import com.nimbusds.jwt.JWTClaimsSet;

import makarosoft.VmsWeb.JwtVerifier.VerifiedToken;

public final class Authz {
    private Authz() {}

    public static boolean isUserOnlyRole(VerifiedToken token) {
        if (token == null || token.getRoles() == null) return false;
        List<String> roles = token.getRoles();
        return roles.size() == 1 && roles.contains("User");
    }

    public static List<String> getAllowedFolders(VerifiedToken token) {
        ArrayList<String> result = new ArrayList<>();
        if (token == null) return result;
        JWTClaimsSet raw = token.getRaw();
        if (raw == null) return result;
        try {
            // AllowedFolder may be multiple claim entries; Nimbus stores multi-valued custom claims
            // inconsistently, so try both list and single string
            List<String> values = raw.getStringListClaim("AllowedFolder");
            if (values != null) {
                for (String v : values) if (v != null && !v.isEmpty()) result.add(v);
            } else {
                String one = raw.getStringClaim("AllowedFolder");
                if (one != null && !one.isEmpty()) result.add(one);
            }
        } catch (ParseException ignored) {
        }
        return result;
    }

    public static boolean isPathAllowed(String pathNormalized, List<String> allowed) {
        if (allowed == null || allowed.isEmpty()) return false;
        // Normalize trailing slash handling for comparison
        String p = normalize(pathNormalized);
        for (String rule : allowed) {
            if (matchesRule(p, rule)) return true;
        }
        return false;
    }

    public static Set<String> allowedDisks(List<String> allowed) {
        HashSet<String> disks = new HashSet<>();
        if (allowed == null) return disks;
        for (String rule : allowed) {
            int idx = rule.indexOf(":/");
            if (idx > 0) {
                String disk = rule.substring(0, idx + 1).toLowerCase(); // keep trailing ':' and lowercase for CI compare
                disks.add(disk);
            }
        }
        return disks;
    }

    private static boolean matchesRule(String path, String rule) {
        String r = normalize(rule);
        boolean wildcard = r.endsWith("/*");
        String base = wildcard ? r.substring(0, r.length() - 2) : r;
        if (wildcard) {
            // Allow exact base or any subpath under it
            if (path.equals(base)) return true;
            if (path.startsWith(base + "/")) return true;
            return false;
        } else {
            return path.equals(base);
        }
    }

    private static String normalize(String text) {
        if (text == null) return "";
        String t = text.trim().toLowerCase();
        // Remove trailing slash except root-like "disk:/"
        if (t.endsWith("/") && !t.endsWith(":/")) {
            t = t.substring(0, t.length() - 1);
        }
        return t;
    }
}


