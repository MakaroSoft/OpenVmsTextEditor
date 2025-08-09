package makarosoft.VmsWeb;

import com.nimbusds.jose.JOSEException;
import com.nimbusds.jose.crypto.RSASSAVerifier;
import com.nimbusds.jose.jwk.JWK;
import com.nimbusds.jose.jwk.KeyType;
import com.nimbusds.jose.jwk.RSAKey;
import com.nimbusds.jwt.JWTClaimsSet;
import com.nimbusds.jwt.SignedJWT;

import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.security.interfaces.RSAPublicKey;
import java.text.ParseException;
import java.time.Instant;
import java.util.Collections;
import java.util.List;

public final class JwtVerifier {
    private final RSAPublicKey publicKey;
    private final String expectedIssuer;
    private final String expectedAudience;
    private final long clockSkewSeconds;

    public JwtVerifier(Path publicKeyPemPath, String expectedIssuer, String expectedAudience, long clockSkewSeconds) {
        this.publicKey = loadPublicKey(publicKeyPemPath);
        this.expectedIssuer = expectedIssuer;
        this.expectedAudience = expectedAudience;
        this.clockSkewSeconds = clockSkewSeconds;
    }

    private static RSAPublicKey loadPublicKey(Path pemPath) {
        try {
            String pem = new String(Files.readAllBytes(pemPath), StandardCharsets.US_ASCII);

            // Nimbus 10.x returns JWK here
            JWK jwk = JWK.parseFromPEMEncodedObjects(pem);

            if (jwk == null || jwk.getKeyType() != KeyType.RSA) {
                throw new IllegalStateException("PEM does not contain an RSA public key");
            }

            RSAKey rsaJwk = (RSAKey) jwk;          // safe after the check
            return rsaJwk.toRSAPublicKey();

        } catch (Exception e) {
            throw new IllegalStateException("Failed to load RSA public key: " + pemPath, e);
        }
    }

    public VerifiedToken verify(String bearerToken) throws SecurityException {
        try {
            SignedJWT jwt = SignedJWT.parse(bearerToken);

            // 1) Signature
            RSASSAVerifier verifier = new RSASSAVerifier(publicKey);
            if (!jwt.verify(verifier)) {
                throw new SecurityException("Invalid signature");
            }

            // 2) Claims
            JWTClaimsSet claims = jwt.getJWTClaimsSet();
            Instant now = Instant.now();

            Instant exp = claims.getExpirationTime() == null ? null : claims.getExpirationTime().toInstant();
            if (exp == null || now.isAfter(exp.plusSeconds(clockSkewSeconds))) {
                throw new SecurityException("Token expired");
            }

            Instant nbf = claims.getNotBeforeTime() == null ? null : claims.getNotBeforeTime().toInstant();
            if (nbf != null && now.isBefore(nbf.minusSeconds(clockSkewSeconds))) {
                throw new SecurityException("Token not yet valid");
            }

            String iss = claims.getIssuer();
            if (iss == null || !iss.equals(expectedIssuer)) {
                throw new SecurityException("Invalid issuer");
            }

            List<String> aud = claims.getAudience();
            if (aud == null || !aud.contains(expectedAudience)) {
                throw new SecurityException("Invalid audience");
            }

            String sub = claims.getSubject();
            String name = getUsername(claims);
            
            List<String> roles = getRoles(
            	    claims,
            	    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            	);
            
            if (roles.size() == 0) {
            	throw new SecurityException("Missing Roles");
            }

            return new VerifiedToken(sub, name, roles, claims);
        } catch (ParseException e) {
            throw new SecurityException("Malformed JWT", e);
        } catch (JOSEException e) {
            throw new SecurityException("Signature verification failed", e);
        }
    }
    
    
    public static List<String> getRoles(JWTClaimsSet claims, String claimName) {
        if (claims == null || claimName == null) {
            return Collections.emptyList();
        }

        try {
            // Try to read as a list first
            List<String> roles = claims.getStringListClaim(claimName);
            if (roles != null) {
                return roles;
            }
        } catch (Exception ignored) {
            // Ignore and try single-string case
        }

        try {
            // Try as a single string
            String singleRole = claims.getStringClaim(claimName);
            if (singleRole != null) {
                return Collections.singletonList(singleRole);
            }
        } catch (Exception ignored) {
        }

        return Collections.emptyList();
    }

    public static String getUsername(JWTClaimsSet claims) throws ParseException {
        // 1) Standard short name
        String v = claims.getStringClaim("name");
        if (v != null && !v.isEmpty()) return v;

        // 2) Common alternates
        v = claims.getStringClaim("preferred_username");
        if (v != null && !v.isEmpty()) return v;

        v = claims.getStringClaim("unique_name");
        if (v != null && !v.isEmpty()) return v;

        // 3) Full WS-Identity URI that .NET sometimes uses
        v = claims.getStringClaim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        if (v != null && !v.isEmpty()) return v;

        // 4) As a last resort, fall back to sub
        return claims.getSubject();
    }
    
    public static class VerifiedToken {
        private final String sub;
        private final String name;
        private final List<String> roles;
        private final JWTClaimsSet raw;

        public VerifiedToken(String sub, String name, List<String> roles, JWTClaimsSet raw) {
            this.sub = sub;
            this.name = name;
            this.roles = roles;
            this.raw = raw;
        }

        public String getSub() { return sub; }
        public String getName() { return name; }
        public List<String> getRoles() { return roles; }
        public JWTClaimsSet getRaw() { return raw; }
    }
}
