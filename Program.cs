using SimpleConsole.Validators;
using Console = SimpleConsole.Console;

namespace AtmApp;

public class Atm
{
    private const int AccountsNumber = 2;
    private const int MaxLoginAttempts = 3;
    private const string ProgramTitle = "CAJERO AUTOMÁTICO";
    private const string ProgramSummary = "Este programa se encarga de simular el funcionamiento de un cajero automático. El cajero permite registrar varias cuentas bancarias, así como realizar distintas operaciones con y entre ellas.";
    
    public static void Main(string[] args)
    {
        var con = new Console(defaultClear: false);
        var accounts = new List<BankAccount?>();
        var mainOptions = new List<string>();
        var accountOptions = new List<string>()
        {
            "Consultar",
            "Depositar",
            "Retirar",
            "Transferir",
            "Cambiar NIP",
            "Cerrar sesión"
        };
        
        con.Greeting(ProgramTitle, ProgramSummary);
        for (var i = 1; i <= AccountsNumber; i++)
        {
            con.Print($"Ingresa los datos para crear una cuenta bancaria ({i}/{AccountsNumber})");
            var account = new BankAccount(
                con.ReadString("Ingresa el nombre del titular de la cuenta", StringValidators.NotBlank),
                con.ReadDouble("Ingresa el saldo inicial de la cuenta", NumericValidators.PositiveOrZero),
                con.ReadString("Ingresa el pin de la cuenta", StringValidators.MinLength(4), StringValidators.MaxLength(4), StringValidators.Pattern(BankAccount.PinPattern))
            );
            accounts.Add(account);
            mainOptions.Add($"Número de cuenta {account.AccountNumber[..4]} {account.AccountNumber[4..]}");
            con.Info($"Cuenta registrada con el número {account.AccountNumber}");
        }
        for (;;)
        {
            MainMenu:
            if (!con.SelectOption("Selecciona una opción", out var account, accounts, mainOptions, "Salir")) break;
            var success = false;
            for (var i = 1; i <= MaxLoginAttempts; i++)
            {
                success = con.ReadString($"Ingresa tu pin para acceder a la cuenta ({i}/{MaxLoginAttempts})") == account!.Pin;
                if (success) break;
            }
            if (!success)
            {
                con.Warning("Número máximo de intentos excedido");
                continue;
            }
            con.ClearIfDefault();
            con.Info($"Hola, {account!.OwnerName}");
            for (;;)
            {
                switch (con.SelectOption("Selecciona una operación", Enumerable.Range(1, accountOptions.Count).ToArray(), accountOptions))
                {
                    case 1:
                        con.Info($"Saldo en la cuenta: ${Math.Round(account.Balance, 7)}");
                        break;
                    case 2:
                        account.Deposit(con.ReadDouble("Ingresa el monto a depositar", NumericValidators.Positive));
                        con.Info("Depósito realizado");
                        break;
                    case 3:
                        account.Withdraw(con.ReadDouble("Ingresa el monto a retirar", NumericValidators.Positive, NumericValidators.Max(account.Balance)));
                        con.Info("Retiro realizado");
                        break;
                    case 4:
                        try
                        {
                            var recipientAccountNumber = con.ReadString("Ingresa el número de cuenta del destinatario").Replace(" ", ""); 
                            var recipientAccount = accounts.Single(a => a!.AccountNumber == recipientAccountNumber);
                            account.Transfer(recipientAccount!, con.ReadDouble("Ingresa el monto a transferir", NumericValidators.Positive, NumericValidators.Max(account.Balance)));
                            con.Info("Transferencia realizada");
                        }
                        catch (Exception)
                        {
                            con.Warning("Cuenta inexistente");
                        }
                        break;
                    case 5:
                        account.Pin = con.ReadString("Ingresa tu nuevo NIP", StringValidators.MinLength(4), StringValidators.MaxLength(4), StringValidators.Pattern(BankAccount.PinPattern));
                        con.Info("Cambio de NIP realizado");
                        break;
                    case 6:
                        con.Info($"¡Hasta luego, {account.OwnerName}!");
                        goto MainMenu;
                }
            }
        }
        con.Farewell("Bye :p");
    }
}