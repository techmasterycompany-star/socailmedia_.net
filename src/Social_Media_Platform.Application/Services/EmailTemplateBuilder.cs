
public static class EmailTemplateBuilder
{
    public static string VerificationEmail(string otp)
    {
        var safeOtp = System.Net.WebUtility.HtmlEncode(otp);

        return $"""
            <!DOCTYPE html>
            <html>
            <body style="margin:0;padding:0;background-color:#f4f6fb;font-family:Arial,sans-serif;color:#1f2937;">
                <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background-color:#f4f6fb;padding:40px 12px;">
                    <tr>
                        <td align="center">
                            <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="max-width:520px;background:#ffffff;border-radius:12px;">
                                <tr>
                                    <td style="padding:36px 32px;text-align:center;">
                                        <h1 style="margin:0 0 12px;color:#4f46e5;font-size:26px;">Social Media Platform</h1>
                                        <h2 style="margin:24px 0 12px;font-size:21px;">Verify your email</h2>
                                        <p style="margin:0 0 24px;color:#6b7280;font-size:15px;line-height:1.7;">
                                            Welcome! Enter the verification code below to activate your account.
                                        </p>
                                        <div style="padding:20px 12px;background:#f3f4f6;border:1px dashed #c7d2fe;border-radius:8px;">
                                            <span style="font-size:32px;font-weight:bold;letter-spacing:8px;color:#4f46e5;">
                                                {safeOtp}
                                            </span>
                                        </div>
                                        <p style="margin:20px 0 0;color:#6b7280;font-size:13px;">
                                            This code expires in 10 minutes.
                                        </p>
                                        <p style="margin:12px 0 0;color:#6b7280;font-size:13px;">
                                            For your security, never share this code with anyone.
                                        </p>
                                        <p style="margin:28px 0 0;color:#9ca3af;font-size:12px;line-height:1.6;">
                                            If you didn't create this account, you can safely ignore this email.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                            <p style="margin:20px 0 0;color:#9ca3af;font-size:12px;">
                                &copy; {DateTime.UtcNow.Year} Social Media Platform
                            </p>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;
    }

    public static string PasswordResetEmail(string otp)
    {
        var safeOtp = System.Net.WebUtility.HtmlEncode(otp);

        return $"""
            <!DOCTYPE html>
            <html>
            <body style="margin:0;padding:0;background-color:#f4f6fb;font-family:Arial,sans-serif;color:#1f2937;">
                <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background-color:#f4f6fb;padding:40px 12px;">
                    <tr>
                        <td align="center">
                            <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="max-width:520px;background:#ffffff;border-radius:12px;">
                                <tr>
                                    <td style="padding:36px 32px;text-align:center;">
                                        <h1 style="margin:0 0 12px;color:#4f46e5;font-size:26px;">Social Media Platform</h1>
                                        <h2 style="margin:24px 0 12px;font-size:21px;">Reset your password</h2>
                                        <p style="margin:0 0 24px;color:#6b7280;font-size:15px;line-height:1.7;">
                                            We received a request to reset your password. Enter the code below to continue.
                                        </p>
                                        <div style="padding:20px 12px;background:#f3f4f6;border:1px dashed #c7d2fe;border-radius:8px;">
                                            <span style="font-size:32px;font-weight:bold;letter-spacing:8px;color:#4f46e5;">
                                                {safeOtp}
                                            </span>
                                        </div>
                                        <p style="margin:20px 0 0;color:#6b7280;font-size:13px;">
                                            This code expires in 10 minutes.
                                        </p>
                                        <p style="margin:12px 0 0;color:#6b7280;font-size:13px;">
                                            For your security, never share this code with anyone.
                                        </p>
                                        <p style="margin:28px 0 0;color:#9ca3af;font-size:12px;line-height:1.6;">
                                            If you didn't request a password reset, you can safely ignore this email.
                                        </p>
                                    </td>
                                </tr>
                            </table>
                            <p style="margin:20px 0 0;color:#9ca3af;font-size:12px;">
                                &copy; {DateTime.UtcNow.Year} Social Media Platform
                            </p>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;
    }
}
