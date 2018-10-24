using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace Neo.SmartContract
{
    public class BookingOrder : Framework.SmartContract
    {
        public static readonly byte[] Owner = "AQRQF27MMwKkzWzDF9tqeg7W7MPZLqAFvX".ToScriptHash();  

        public static Object Main(string operation, params object[] args)
        {
            if (Runtime.Trigger == TriggerType.Verification)
            {
                if (Owner.Length == 20)
                {
                    // if param Owner is script hash
                    return Runtime.CheckWitness(Owner);
                }
                else if (Owner.Length == 33)
                {
                    // if param Owner is public key
                    byte[] signature = operation.AsByteArray();
                   
                    return VerifySignature(signature, Owner);
                }
            }
            else if (Runtime.Trigger == TriggerType.Application)
            {
                if (operation == "query")                   return QueryOrder((string)args[0]);
                else if (operation == "create")             return CreateOrder((string)args[0], (string)args[1]);
                else if (operation == "createMultiple")     return CreateOrderMultiple(args);
                else if (operation == "putMultiple")        return PutOrderMultiple(args);
                else if (operation == "update")             return UpdateOrder((string)args[0], (string)args[1]);
                else if (operation == "setTokenName")       return SetTokenName((string)args[0]);
                else if (operation == "getTokenName")       return GetTokenName();
                else if (operation == "migrateContract")    return UpgradeContract((byte[])args[0], (string)args[1], (string)args[2], (string)args[3], (string)args[4], (string)args[5]);
                else if (operation == "destroyContract")    return DestroyContract();
            }
           
            return false;
        }
       
        /**
         * Store multiple order data to Storage
         */
        private static object CreateOrderMultiple(object[] args)
        {
            if (!Runtime.CheckWitness(Owner))
            {
                return false;
            }

            int len = args.Length;
            if (len % 2 != 0)
            {
                return false;
            }

            for (int i = 0; i < len; i = i + 2)
            {
                string key = (string)args[i];
                string value = (string)args[i + 1];
                Runtime.Log("key " + key);
                Runtime.Log("val " + value);

                Storage.Put(Storage.CurrentContext, key, value);
            }

            return true;
        }

        /**
         * Put multiple order data to Storage
         */
        private static object PutOrderMultiple(object[] args)
        {
            if (!Runtime.CheckWitness(Owner))
            {
                return false;
            }

            int len = args.Length;
            if (len % 2 != 0)
            {
                return false;
            }

            for (int i = 0; i < len; i = i + 2)
            {
                string key = (string)args[i];
                string value = (string)args[i + 1];
                Runtime.Log("key " + key);
                Runtime.Log("val " + value);

                Storage.Put(Storage.CurrentContext, key, value);
            }

            return true;
        }

        /**
        * Update order data in Storage
        */
        private static object UpdateOrder(string domain, string value)
        {
            if (!Runtime.CheckWitness(Owner))
            {
                return false;
            }

            byte[] v = Storage.Get(Storage.CurrentContext, domain);
            if (v == null) return false;

            Storage.Delete(Storage.CurrentContext, domain);
            Storage.Put(Storage.CurrentContext, domain, value);
            return true;
        }

        /**
        * Store order data to Storage
        */
        private static object CreateOrder(string domain, string value)
        {
            if (!Runtime.CheckWitness(Owner))
            {
                return false;
            }

            byte[] v = Storage.Get(Storage.CurrentContext, domain);
            if (v != null) return false;

            Storage.Put(Storage.CurrentContext, domain, value);
            return true;
        }

        /**
        * Query order data from Storage
        */
        private static object QueryOrder(string domain)
        {
            byte[] v = Storage.Get(Storage.CurrentContext, domain);
            if (v == null) return false;

            return v;
        }

        /**
         *  Migrate contract to new one 
         */
        private static bool UpgradeContract(byte[] newScript, string newName, string newVersion, string newAuthor, string newEmail, string newDescription)
        {
            if (!Runtime.CheckWitness(Owner))
            {
                return false;
            }

            Runtime.Notify("Starting Upgrade");
            Contract.Migrate(newScript, new byte[] { 0x07, 0x10 }, 0x05, true, newName, newVersion, newAuthor, newEmail, newDescription);
            Runtime.Notify("Upgrade Complete");
            return true;
        }

        /**
         *  Destroy contract 
         */
        private static bool DestroyContract()
        {
            if (!Runtime.CheckWitness(Owner))
            {
                return false;
            }

            Runtime.Notify("Starting Destroy");
            Contract.Destroy();
            Runtime.Notify("Destroy Complete");
            return true;
        }

        /**
         * Set name for using token
         */
        private static object SetTokenName(string tokenName)
        {
            if (!Runtime.CheckWitness(Owner))
            {
                return false;
            }

            Storage.Put(Storage.CurrentContext, "tokenName", tokenName);
            return true;
        }

        /**
         * Get using token name
         */
        private static object GetTokenName()
        {
            return Storage.Get(Storage.CurrentContext, "tokenName");
        }
    }
}

