using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class NoNameException : Exception
    {
        public NoNameException()
        {

        }
        public NoNameException(string message) : base(message)
        {

        }
    }
    class InvalidNameException : Exception
    {
        public InvalidNameException()
        {

        }
        public InvalidNameException(string message) : base(message)
        {

        }
    }

    public abstract class Account
    {
        public enum AccountStatus
        {
            Active,
            Inactive,
            Closed
        }

        private AccountStatus _Status = AccountStatus.Active;

        public AccountStatus Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
            }
        }


        public DateTime _AccountOpening = DateTime.Now;

        public DateTime AccountOpening
        {
            get
            {
                return this._AccountOpening;
            }
        }

        public string AccountType { get; protected set; }

        public List<Owner> Owners { get; set; }
        protected double Balance { get; set; }
        public long AccountNumber { get; protected set; }

        public virtual string PrintAccount()
        {
            return AccountNumber.ToString() + ": " + Balance + " |Type: " + AccountType + " |Status: " + Status.ToString();
        }

        public void CloseAccount()
        {

            this.Status = AccountStatus.Closed;
        }

        public void DeactivateAccount()
        {
            this.Status = AccountStatus.Inactive;
        }

        public void Deposit(double balance)
        {
            Balance += balance;
        }
        public double GetBalance()
        {
            return this.Balance;
        }

        public void Withdraw(double balance)
        {
            if (balance <= Balance)
            {
                Balance -= balance;
            }
            else
            {
                throw new ArgumentException("Not enough funds for this transaction!");
            }

        }

        public string PrintAllOwners()
        {
            string output = string.Empty;
            output += "Account : " + this.AccountNumber.ToString() + "\r\n" + "Balance : " + this.Balance + "\r\n" + "Open Since : " + this.AccountOpening + "\r\n----------------------------------------\r\n";
            foreach (Owner owner in Owners)
            {
                output += owner.ToString() + "\r\n";
            }
            return output;
        }
    }



    public class Owner
    {
        public string FirstName { get; set; }


        public string LastName { get; set; }


        public string FullName
        {
            get
            {
                return this.LastName + ", " + this.FirstName;
            }
        }
        public DateTime DateOfBirth { get; set; }
        public string SocialSecurityNumber { get; set; }
        public List<Account> MyAccounts { get; set; }


        public Owner(string firstName, string lastName, string dateOfBirth, string socialSecurityNum)
        {
            MyAccounts = new List<Account>();
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = DateTime.Parse(dateOfBirth);
            SocialSecurityNumber = socialSecurityNum;
        }
        public void RemoveFromMyAccounts(Account a)
        {
            this.MyAccounts.Remove(a);
        }
        public string PrintAllAccounts()
        {
            string output = this.ToString() + "\r\n";

            foreach (Account a in MyAccounts)
            {
                output += a.PrintAccount() + "\r\n";
            }

            return output + "\r\n----------------------------------------\r\n";
        }
        public new bool Equals(object obj)
        {
            Owner x = obj as Owner;
            if (x != null)
            {
                if ((x.FirstName == FirstName && x.LastName == LastName) || (x.SocialSecurityNumber == SocialSecurityNumber))
                {
                    return true;
                }
            }

            return false;
        }


        public override string ToString()
        {
            string output = string.Empty;
            output += this.FullName + " | DOB: " + this.DateOfBirth.ToString("MM/dd/yyyy");
            return output;
        }
    }

    public class CheckingAccount : Account
    {
        public static long AccountBase = 1000000000;
        public CheckingAccount(Owner owner, double startingBalance)
        {
            owner.MyAccounts.Add(this);
            this.Balance = startingBalance;
            this.Owners = new List<Owner>();
            this.Owners.Add(owner);
            this.AccountNumber++;
            CheckingAccount.AccountBase++;
            this.AccountNumber = CheckingAccount.AccountBase;
            this.AccountType = "Checking";
        }

        public CheckingAccount(Owner[] owners, double startingBalance)
        {
            foreach (Owner owner in owners)
            {
                owner.MyAccounts.Add(this);
            }
            this.AccountNumber++;
            this.Balance = startingBalance;
            this.Owners = new List<Owner>();
            this.Owners.AddRange(owners);
            CheckingAccount.AccountBase++;
            this.AccountNumber = CheckingAccount.AccountBase;
            this.AccountType = "Checking";
        }
    }

    public class SavingsAccount : Account
    {
        public static long AccountBase = 2000000000;

        public double InterestRate = 1.5;

        public SavingsAccount(Owner owner, double startingBalance)
        {
            owner.MyAccounts.Add(this);
            this.Balance = startingBalance;
            this.Owners = new List<Owner>();
            this.Owners.Add(owner);
            SavingsAccount.AccountBase++;
            this.AccountNumber = SavingsAccount.AccountBase;
            this.AccountType = "Saving";
        }

        public SavingsAccount(double startingBalance, params Owner[] owners)
        {
            foreach (Owner owner in owners)
            {
                owner.MyAccounts.Add(this);
            }
            this.Balance = startingBalance;
            this.Owners = new List<Owner>();
            this.Owners.AddRange(owners);
            SavingsAccount.AccountBase++;
            this.AccountNumber = SavingsAccount.AccountBase;
            this.AccountType = "Saving";
        }
        public override string PrintAccount()
        {
            return AccountNumber.ToString() + ": " + Balance + "  |Type: " + AccountType + " |Interest Rate: " + InterestRate + " |Status: " + Status.ToString();
        }
    }

    class Program
    {
        static List<Account> accounts = new List<Account>();
        static List<Owner> totalowners = new List<Owner>();

        static void Main(string[] args)
        {
            char option = 'z';

            Dictionary<Account, List<Owner>> CompleteList = new Dictionary<Account, List<Owner>>();


            while (option != 'x' && option != 'X')
            {

                Console.WriteLine("1: Create Savings account.");
                Console.WriteLine("2: Create Checking account.");
                Console.WriteLine("3: Print the list of all acounts nts.");
                Console.WriteLine("4: Search Existing Accounts By Owner");
                Console.WriteLine("5: Make a Deposit");
                Console.WriteLine("6: Withdraw Funds");
                Console.WriteLine("7: Check the Balance ");
                Console.WriteLine("8: Close the Account");
                Console.WriteLine("9: Check for the total amount of funds in the bank");
                Console.WriteLine("X: Exit.");

                option = Console.ReadKey().KeyChar;
                Console.WriteLine();

                switch (option)
                {
                    case '1':
                        {
                            Console.Clear();
                            List<Owner> owners = new List<Owner>();
                            owners.Add(GetOwnerInformationFromConsole());

                            char getMoreOwners = 'z';


                            while (getMoreOwners != 'n')
                            {
                                Console.Write("Add more owners? (y/n): ");
                                getMoreOwners = Console.ReadKey().KeyChar;
                                Console.WriteLine();
                                if (getMoreOwners == 'y')
                                {
                                    owners.Add(GetOwnerInformationFromConsole());
                                }
                                else if (getMoreOwners != 'n')
                                {
                                    Console.WriteLine("Please select either 'y' or 'n' from the option menu.");
                                }
                            }

                            double balance = GetBalanceInformationFromConsole();
                            Account acc = new SavingsAccount(balance, owners.ToArray());
                            accounts.Add(acc);
                            foreach (Owner o in owners)
                            {
                                if (!totalowners.Contains(o))
                                {
                                    totalowners.Add(o);
                                }
                            }
                            OpenAccountMessage(acc);
                        }

                        break;
                    case '2':
                        {
                            Console.Clear();
                            List<Owner> owners = new List<Owner>();
                            owners.Add(GetOwnerInformationFromConsole());

                            char getMoreOwners = 'z';


                            while (getMoreOwners != 'n')
                            {
                                Console.Write("Add more owners? (y/n): ");
                                getMoreOwners = Console.ReadKey().KeyChar;
                                Console.WriteLine();
                                if (getMoreOwners == 'y')
                                {
                                    owners.Add(GetOwnerInformationFromConsole());
                                }
                                else if (getMoreOwners != 'n')
                                {
                                    Console.WriteLine("Please select either 'y' or 'n' from the option menu.");
                                }
                            }
                            double balance = GetBalanceInformationFromConsole();
                            Account acc = new CheckingAccount(owners.ToArray(), balance);

                            CompleteList.Add(acc, owners);
                            accounts.Add(acc);
                            foreach (Owner o in owners)
                            {
                                if (!totalowners.Contains(o))
                                {
                                    totalowners.Add(o);
                                }
                            }
                            OpenAccountMessage(acc);
                        }

                        break;

                    case '3':
                        {
                            Console.Clear();
                            foreach (Account acc in accounts)
                            {
                                Console.WriteLine();
                                Console.WriteLine(acc.PrintAllOwners());

                            }

                        }
                        break;
                    case '4':
                        {
                            Console.Clear();
                            object getf = GetOwnerInformationFromConsole();
                            foreach (Owner todo in totalowners)
                                if (todo.Equals(getf))
                                {
                                    Console.WriteLine();
                                    Console.Clear();
                                    Console.WriteLine(todo.PrintAllAccounts());
                                }

                        }
                        break;

                    case '5':
                        {
                            Console.Clear();
                            Account acc = GetAccountNumberFromConsole();
                            if (acc != null)
                            {
                                acc.Deposit(GetAmountFromConsole());
                            }

                        }
                        break;
                    case '6':
                        {
                            Console.Clear();
                            Account acc = GetAccountNumberFromConsole();
                            if (acc != null)
                            {
                                char tryOtherAmount = 'y';
                                do
                                {
                                    try
                                    {
                                        if (tryOtherAmount == 'y')
                                        {
                                            acc.Withdraw(GetAmountFromConsole());
                                        }
                                        else if (tryOtherAmount != 'n')
                                        {
                                            Console.WriteLine("Please select either 'y' or 'n' from the option menu.");
                                        }
                                        tryOtherAmount = 'n';
                                    }
                                    catch (ArgumentException neg)
                                    {
                                        Console.WriteLine(neg.Message + "\r\n");
                                        Console.Write("Try Other Amount? (y/n) ");
                                        tryOtherAmount = Console.ReadKey().KeyChar;
                                        Console.WriteLine();
                                    }

                                } while (tryOtherAmount != 'n');
                                Console.WriteLine();
                            }

                        }
                        break;
                    case '7':
                        {
                            Console.Clear();
                            Account acc = GetAccountNumberFromConsole();
                            if (acc != null)
                            {
                                Console.WriteLine("Your balance is : {0} \r\n", acc.GetBalance());
                            }
                        }
                        break;
                    case '8':
                        {
                            Console.Clear();
                            Account acc = GetAccountNumberFromConsole();
                            if (acc != null)
                            {
                                acc.Withdraw(acc.GetBalance());
                                foreach (Owner o in totalowners)
                                {
                                    if (totalowners.Contains(o))
                                    {
                                        o.RemoveFromMyAccounts(acc);
                                    }
                                }
                                accounts.Remove(acc);
                                acc.CloseAccount();

                                CloseAccountMessage(acc);
                            }

                        }
                        break;
                    case '9':
                        {
                            Console.Clear();
                            double totalFundsInBank = 0;
                            foreach (Account a in accounts)
                            {
                                totalFundsInBank += a.GetBalance();
                            }

                            Console.WriteLine("Total amount of funds in the bank: {0} \r\n", totalFundsInBank);
                        }
                        break;
                }

            }

        }

        public static Owner GetOwnerInformationFromConsole()
        {
            bool valid = false;
            string lastName = null;
            while (!valid)
            {
                try
                {
                    Console.Write("Enter Last name: ");
                    lastName = Console.ReadLine();
                    ValidateNameInput(lastName);
                    valid = true;
                }
                catch (NoNameException na)
                {
                    Console.WriteLine(na.Message);
                    valid = false;
                }
                catch (InvalidNameException es)
                {
                    Console.WriteLine(es.Message);
                    valid = false;
                }
            }

            valid = false;


            string firstName = null;
            while (!valid)
            {
                try
                {
                    Console.Write("Enter First name: ");
                    firstName = Console.ReadLine();
                    ValidateNameInput(firstName);
                    valid = true;
                }
                catch (NoNameException na)
                {
                    Console.WriteLine(na.Message);
                    valid = false;
                }
                catch (InvalidNameException es)
                {
                    Console.WriteLine(es.Message);
                    valid = false;
                }
            }

            string dateOfBirth = "";

            valid = false;
            while (!valid)
            {
                try
                {
                    Console.Write("Enter Date of Birth: ");
                    dateOfBirth = Console.ReadLine();
                    DateTime.Parse(dateOfBirth);
                    valid = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Enter a vaild date of birth in the format: MM/DD/YYYY");
                    valid = false;
                }
            }

            Console.Write("Enter SSN: ");
            string ssn = Console.ReadLine();

            Owner output = null;

            foreach (Owner o in totalowners)
            {
                if (o.FirstName == firstName &&
                    o.LastName == lastName &&
                    o.DateOfBirth == DateTime.Parse(dateOfBirth) &&
                    o.SocialSecurityNumber == ssn)
                {
                    output = o;
                }
            }

            if (output == null)
                return new Owner(firstName, lastName, dateOfBirth, ssn);
            else return output;

        }

        public static double GetBalanceInformationFromConsole()
        {
            bool valid = false;
            double balance = 0;
            while (!valid)
            {
                try
                {
                    Console.Write("Enter Starting Balance: ");
                    string startingBalance = Console.ReadLine();
                    balance = double.Parse(startingBalance);
                    valid = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Enter a valid number in form of XXX.XX");
                    valid = false;
                }

            }

            return balance;
        }
        public static double GetAmountFromConsole()
        {
            bool valid = false;
            double amount = 0;
            while (!valid)
            {
                try
                {
                    Console.Write("Enter the Amount: ");
                    string inputAmount = Console.ReadLine();
                    amount = double.Parse(inputAmount);
                    valid = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Enter a valid number in form of XXX.XX");
                    valid = false;
                }

            }

            return amount;
        }
        public static Account GetAccountNumberFromConsole()
        {
            bool valid = false;
            int accNumber = 0;
            Account output = null;
            while (!valid)
            {
                try
                {
                    Console.Write("Enter Account Number: ");
                    string inputAccNumber = Console.ReadLine();
                    accNumber = int.Parse(inputAccNumber);

                    valid = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Enter a valid Account number it should be 10 digits");
                    valid = false;
                }
                bool validoutput = false;
                foreach (Account a in accounts)
                {
                    if (a.AccountNumber == accNumber)
                    {
                        output = a;
                        validoutput = true;
                    }
                }
                if (validoutput == false)
                {
                    Console.WriteLine("Account not Found.");
                }

            }
            return output;
        }

        public static void OpenAccountMessage(Account acc)
        {
            Console.WriteLine();
            Console.WriteLine("{0} account was succesfully opened! ", acc.AccountType);
            Console.WriteLine("Account number is: {0} \r\n", acc.AccountNumber);
        }
        public static void CloseAccountMessage(Account acc)
        {
            Console.WriteLine();
            Console.WriteLine("{0} account was succesfully closed! ", acc.AccountType);
            Console.WriteLine("Account number is: {0} \r\n", acc.AccountNumber);
        }
        private static void ValidateNameInput(string name)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name))
            {
                throw new NoNameException("Empty Input. Try again ! ");
            }
            bool val = true;
            for (int i = 0; i < name.Length; i++)
            {
                val = Char.IsLetter(name[i]) & val;
            }

            if (!val)
            {
                throw new InvalidNameException("Invalid Input. Try Again !");
            }
        }

    }
}