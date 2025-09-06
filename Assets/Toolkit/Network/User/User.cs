using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Network {
    [System.Serializable]
    public class User {
        #region Classes

        [System.Serializable]
        public class Service {
            #region Variables

            [SerializeField] private ServiceType serviceType = ServiceType.None;
            [SerializeField] private string serviceCustom = "";
            [SerializeField] private string token = "";
            [SerializeField] private string secret = "";

            #endregion

            #region Properties

            public ServiceType ServiceType => serviceType;
            public string ServiceCustomType => serviceCustom;
            public string Token => token;
            public string Secret => secret;

            public static Service Empty => new Service();

            #endregion

            #region Constructor

            public Service() { }

            public Service(ServiceType type, string token, string secret = null) {
                this.serviceType = type;
                this.serviceCustom = null;
                this.token = token;
                this.secret = secret;
            }

            #endregion

            #region Create

            public static Service CreateCustom(string customService, string token, string secret = null) {
                var s = new Service(ServiceType.Custom, token, secret);
                s.serviceCustom = customService;
                return s;
            }

            #endregion

            #region Save / Load

            public void Save(Toolkit.IO.TMLNode parent) {
                var node = parent.AddNode(serviceType.ToString());
                if(serviceType == ServiceType.Custom)
                    node.AddProperty("custom", serviceCustom);
                node.AddProperty("token", token);
                node.AddProperty("secret", secret);
            }

            public void Load(Toolkit.IO.TMLNode serviceTypeNode) {
                var t = serviceTypeNode.Name;
                if(FastEnum.TryParse<ServiceType>(t, out ServiceType result)) {
                    this.serviceType = result;
                    serviceCustom = serviceTypeNode.GetString("custom");
                    token = serviceTypeNode.GetString("token");
                    secret = serviceTypeNode.GetString("secret");
                }
            }

            #endregion
        }

        public enum ServiceType : byte {
            None = 0,
            Mail = 1,
            DeviceID = 2,
            Custom = 4,
            // PC Platforms
            Steam = 10,
            EpicGames = 11,
            // Mobile Providers
            Google = 20,
            Apple = 21,
            // VR Platforms
            Meta_Quest = 30
        }

        #endregion

        #region Variables

        private const string TAG = "[User] - ";
        private const int DEFAULT_CONFIRMATION_CODE_LENGTH = 6;

        // User
        [SerializeField] private string username = "";
        [SerializeField] private int usernameSuffix = 0;
        [SerializeField] private string mail = "";
        [SerializeField, Readonly] private string playerId_text; // used for debugging purposes
        private Guid playerId;

        // Password
        [SerializeField] private byte[] password;
        [SerializeField] private string salt;
        [SerializeField] private Password.SecurityMode mode = Password.SecurityMode.None;
        [SerializeField] private Password.SecurityType type = Password.SecurityType.None;

        // Linked Service
        [SerializeField] private List<Service> services = new List<Service>();

        // Login Data
        [SerializeField] private int loginAttempts = 0;
        [SerializeField] private long loginAttemptTime = 0L;
        [SerializeField] private long lastSuccessfulLogin = 0L;

        #endregion

        #region Properties

        public string Username => username;
        public string FullUsername => $"{username}#{usernameSuffix}";
        public string Mail => mail;
        public Guid PlayerId {
            get => playerId;
            private set {
                playerId = value;
                playerId_text = value.ToString();
            }
        }
        public string PlayerId_Text => playerId_text;

        public byte[] PasswordData => password;
        public string PasswordSalt => salt;
        public Password.SecurityMode PasswordMode => mode;
        public Password.SecurityType PasswordType => type;

        public IReadOnlyList<Service> Services => services;

        #endregion

        #region Password Check

        public bool Login(string password) {
            var t = System.DateTime.UtcNow.Ticks;
            loginAttempts++;
            if(loginAttempts > 5 && TKTimeSpan.FromTicks(loginAttemptTime - t).TotalMinutes < 5f) {
                loginAttemptTime = t;
                Debug.LogWarning(TAG + $"Server attmpting to login too many times ({loginAttempts}) in past minute.");
                return false;
            }
            loginAttemptTime = t;
            var result = Password.Compare(this, password);
            if(result) {
                loginAttempts = 0;
                lastSuccessfulLogin = t;
            }
            return result;
        }

        public bool Login_PreHashed(byte[] bytes) {
            var t = System.DateTime.UtcNow.Ticks;
            loginAttempts++;
            if(loginAttempts > 5 && TKTimeSpan.FromTicks(loginAttemptTime - t).TotalMinutes < 5f) {
                loginAttemptTime = t;
                Debug.LogWarning(TAG + $"Server attmpting to login too many times ({loginAttempts}) in past minute.");
                return false;
            }
            loginAttemptTime = t;
            var result = true;
            for(int i = 0, length = password.Length; i < length; i++) {
                if(bytes[i] != password[i])
                    result = false;
            }
            if(result) {
                loginAttempts = 0;
                lastSuccessfulLogin = t;
            }
            return result;
        }

        #endregion

        #region Constructor

        public User() {
            PlayerId = Guid.NewGuid();
        }
        public User(byte[] data) {
            // Implement TML Node parsing from binary
            throw new Exception("Not implemented!");
        }

        #endregion

        #region Creation

        public static User CreateFromTML(string tml) {
            User u = new User();
            u.LoadFromTML(tml);
            return u;
        }

        public static User Create(string username, int suffix, string password) {
            User u = new User();

            // Username
            u.username = username;
            u.usernameSuffix = suffix;
            // Guid is generated in default constructor
            // u.PlayerId = Guid.NewGuid();

            // Password
            u.salt = Password.Salt.URL_Safe();
            u.password = Password.Create(password, u.salt);
            u.type = Password.Type;
            u.mode = Password.Mode;

            return u;
        }

        #endregion

        #region Mail

        public void ConnectMail(string mail) {
            if(!string.IsNullOrEmpty(this.mail)) {
                Debug.LogWarning(TAG + $"Attempting to connect mail while already have a mail linked '{FullUsername}' at '{mail}'");
                return;
            }
            if(IsMailValid(mail)) {
                this.mail = mail;
            }
        }

        public void ReplaceMail(string mail) {
            if(IsMailValid(mail))
                this.mail = mail;
        }

        public static bool IsMailValid(string mail) {
            try {
                var ma = new System.Net.Mail.MailAddress(mail);
                return true;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
            }
            return false;
        }

        #endregion

        #region Services

        public bool AddService(ServiceType type, string token, string secret = null)
            => AddService(new Service(type, token, secret));

        public bool AddService(Service service) {
            if(service == null) {
                Debug.LogError(TAG + "Service is null");
                return false;
            }
            if(HasService(service.ServiceType)) {
                Debug.LogError(TAG + "Service already exist");
                return false;
            }
            services.Add(service);
            return true;
        }

        public bool HasService(ServiceType type) {
            foreach(Service service in services)
                if(service.ServiceType == type)
                    return true;
            return false;
        }

        public bool TryGetService(ServiceType type, out Service service) {
            foreach(var s in services) {
                if(s.ServiceType == type) {
                    service = s;
                    return true;
                }
            }
            service = null;
            return false;
        }

        public bool RemoveService(Service service)
            => services.Remove(service);

        public bool RemoveService(ServiceType type) {
            for(int i = services.Count - 1; i >= 0; i--) {
                if(services[i].ServiceType == type) {
                    services.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Print

        public override string ToString() {
            return playerId.ToString();
        }

        public string ToStringDetailed() {
            return $"{username}#{usernameSuffix}";
        }

        #endregion

        #region Save / Load

        public void LoadFromTML(string text) {
            try {
                var root = Toolkit.IO.TML.TMLParser.Parse(text);
                Toolkit.IO.TML.TMLUtility.Debug("user", root);
                if(root.TryGetNode("userData", out IO.TMLNode data)) {
                    username = data.GetString("username");
                    usernameSuffix = data.GetInt("suffix");
                    mail = data.GetString("mail");
                    playerId_text = data.GetString("guid");
                    Guid.TryParse(playerId_text, out playerId);
                }
                if(root.TryGetNode("password", out IO.TMLNode pass)) {
                    var val = pass.GetString("value");
                    var splitted = val.Split('.');
                    password = new byte[splitted.Length];
                    for(int i = 0, length = password.Length; i < length; i++) {
                        byte.TryParse(splitted[i], out password[i]);
                    }
                    salt = pass.GetString("salt");
                    mode = pass.GetString("mode")?.ToEnum(Password.SecurityMode.Salt_Pepper) ?? Password.SecurityMode.Salt_Pepper;
                    type = pass.GetString("type")?.ToEnum(Password.SecurityType.SHA256) ?? Password.SecurityType.SHA256;
                }
                if(root.TryGetNode("services", out IO.TMLNode serviceNode)) {
                    services.Clear();
                    foreach(var n in serviceNode.Nodes) {
                        var s = new Service();
                        s.Load(n);
                        services.Add(s);
                    }
                }
            }
            catch(System.Exception e) {
                Debug.LogError(TAG + "Unable to load from Toolkit Markdown Language\n" + text);
                Debug.LogException(e);
            }
        }

        public string SaveToTML() {
            Toolkit.IO.TMLNode node = new IO.TMLNode("root");

            // Store base data
            var data = node.AddNode("userData");
            data.AddProperty("username", username);
            data.AddProperty("suffix", usernameSuffix);
            data.AddProperty("mail", mail);
            data.AddProperty("guid", playerId_text);

            // Store password
            var pass = node.AddNode("password");
            pass.AddProperty("value", string.Join('.', password));
            pass.AddProperty("salt", salt);
            pass.AddProperty("mode", mode);
            pass.AddProperty("type", type);

            // Services
            var serviceNode = node.AddNode("services");
            serviceNode.AddProperty("count", this.services.Count);
            foreach(var s in services) {
                s.Save(serviceNode);
            }

            return Toolkit.IO.TML.TMLParser.ToString(node, true);
        }

        #endregion
    }
}
