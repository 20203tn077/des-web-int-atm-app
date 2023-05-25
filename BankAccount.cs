using System.Text.RegularExpressions;

namespace AtmApp;

public class NotEnoughBalanceException : Exception {}
public class InvalidAmountException : Exception {}
public class InvalidPinException : Exception {}

public class BankAccount
{
    public static Regex PinPattern = new Regex("^\\d{4}$");
    public string OwnerName { get; private set; }
    
    public readonly string AccountNumber;
    private double _balance;
    private string _pin = "0000";

    public double Balance
    {
        get => _balance;
        private set
        {
            ValidateBalance(value);
            _balance = value;
        }
    }

    public string Pin
    {
        get => _pin;
        set
        {
            ValidatePin(value);
            _pin = value;
        }
    }

    public BankAccount(string ownerName, double initialBalance, string pin)
    {
        OwnerName = ownerName;
        Balance = initialBalance;
        Pin = pin;
        AccountNumber = new Random().NextInt64(99999999).ToString().PadLeft(8, '0');
    }

    public void Deposit(double amount)
    {
        ValidateAmount(amount);
        Balance += amount;
    }

    public void Withdraw(double amount)
    {
        ValidateAmount(amount);
        ValidateBalanceAvailability(amount);
        Balance -= amount;
    }

    public void Transfer(BankAccount recipient, double amount)
    {
        ValidateAmount(amount);
        Balance -= amount;
        recipient.Balance += amount;
    }
    
    private void ValidatePin(string pin)
    {
        if (!PinPattern.IsMatch(pin)) throw new InvalidAmountException();
    }
    
    private void ValidateAmount(double amount)
    {
        if (amount <= 0) throw new InvalidAmountException();
    }
    
    private void ValidateBalance(double amount)
    {
        if (amount < 0) throw new InvalidAmountException();
    }

    private void ValidateBalanceAvailability(double amount)
    {
        if (amount > Balance) throw new NotEnoughBalanceException();
    }
}