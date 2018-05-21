namespace BizyBoard.Core.Helpers
{
    public static class Constants
    {
        public static class Strings
        {
            public static class JwtClaimIdentifiers
            {
                public const string Role = "rol", Id = "id", Tenant = "tenid", Company = "comp";
            }

            public const string WinBizEncryptionKey =
                "BgIAAACkAABSU0ExAAQAAAEAAQBZ3myd6ZQA0tUXZ3gIzu1sQ7larRfM5KFiYbkgWk+jw2VEWpxpNNfDw8M3MIIbbDeUG02y/ZW+XFqyMA/87kiGt9eqd9Q2q3rRgl3nWoVfDnRAPR4oENfdXiq5oLW3VmSKtcBl2KzBCi/J6bbaKmtoLlnvYMfDWzkE3O1mZrouzA==";

            public static class Errors
            {
                public static (string code, string message) Base = ("base_error", "Un erreur est survenue...");
                public static (string code, string message) EmptyEmail = ("emtpy_email", "L'email n'est pas renseigné.");
                public static (string code, string message) NoWinBizFolder = ("no_winbiz_folder", "Pas de dossier ouvert dans WinBIZ Cloud");
                public static (string code, string message) LoginFailure = ("login_failure", "Mot de passe ou nom d'utilisateur incorrect.");
                public static (string code, string message) DuplicateEmail = ("duplicate_email", "Email déjà existant.");
                public static (string code, string message) PasswordResetUpdateError = ("password_reset_update_error", "Erreur durant la mise à jour du mot de passe.");
                public static (string code, string message) UserNotFound = ("user_not_found", "User introuvable dans la base de données");
            }

            public static class Success
            {
                public static (string code, string message) PasswordReset = ("password_reset", "Un email a été envoyé avec les instructions à suivre.");
                public static (string code, string message) PasswordResetUpdate = ("password_reset_update", "Votre mot de passe a été modifié.");
            }
        }
    }
}
