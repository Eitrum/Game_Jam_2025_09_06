using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;

namespace Toolkit.Network {
    public static class WhatIsMyIP {

        #region Variables

        public const float REQUEST_TIMEOUT = 10f;

        #endregion

        #region PublicIP

        private static System.Net.IPAddress cachedPublicIP = null;
        private static bool isAttemptingToCachePublicIP;

        public static void PreparePublicIP() {
            CachePublicIPAsync();
        }

        public static void RemovePublicIPCache(bool recache = false) {
            cachedPublicIP = null;
            if(recache)
                PreparePublicIP();
        }

        public static System.Net.IPAddress GetPublicIP() {
            TryGetPublicIP(out var address);
            return address;
        }

        public static bool TryGetPublicIP(out System.Net.IPAddress address) {
            if(cachedPublicIP == null || !isAttemptingToCachePublicIP)
                CachePublicIPAsync();
            address = cachedPublicIP;
            return address != null;
        }

        public static async System.Threading.Tasks.Task<System.Net.IPAddress> GetPublicIPAsync(System.Threading.CancellationToken cancellation = default) {
            if(cachedPublicIP != null)
                return cachedPublicIP;

            CachePublicIPAsync();

            while(isAttemptingToCachePublicIP) {
                if(cancellation.IsCancellationRequested) {
                    return cachedPublicIP;
                }
                await System.Threading.Tasks.Task.Yield();
            }

            return cachedPublicIP;
        }

        private static async void CachePublicIPAsync() {
            if(isAttemptingToCachePublicIP)
                return;

            try {
                System.Func<System.Threading.Tasks.Task<System.Net.IPAddress>>[] CacheFunctions = {
                    GetPublicIP_httpbin,
                    GetPublicIP_dyndns,
                    GetPublicIP_ipify,
                };

                CacheFunctions.Shuffle(3);

                isAttemptingToCachePublicIP = true;
                for(int i = 0; i < 9; i++) {
                    cachedPublicIP = await CacheFunctions[i % 3]();
                    if(cachedPublicIP != null)
                        return;
                }
            }
            catch(System.Exception e) {
                Debug.LogException(e);
            }
            finally {
                isAttemptingToCachePublicIP = false;
            }
        }

        #endregion

        #region api.ipify.org

        private static async System.Threading.Tasks.Task<System.Net.IPAddress> GetPublicIP_ipify() {
            try {
                var request = SimpleHttpClient.GetContent("https://api.ipify.org/", REQUEST_TIMEOUT);
                var result = await request.Wait(); if(result == SimpleHttpClient.Request.State.None) {
                    return default;
                }
                var content = await request.Content.ReadAsStringAsync();
                System.Net.IPAddress.TryParse(content, out var address);
                return address;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return default;
            }
        }

        #endregion

        #region checkip.dyndns

        private static async System.Threading.Tasks.Task<System.Net.IPAddress> GetPublicIP_dyndns() {
            try {
                var request = SimpleHttpClient.GetContent("http://checkip.dyndns.org/", REQUEST_TIMEOUT);
                var result = await request.Wait(); if(result == SimpleHttpClient.Request.State.None) {
                    return default;
                }
                var content = await request.Content.ReadAsStringAsync();
                var value = content.Substring(76, content.Length - 92);
                System.Net.IPAddress.TryParse(value, out var address);
                return address;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return default;
            }
        }

        #endregion

        #region httpbin.org/ip

        private static async System.Threading.Tasks.Task<System.Net.IPAddress> GetPublicIP_httpbin() {
            try {
                var request = SimpleHttpClient.GetContent("http://httpbin.org/ip", REQUEST_TIMEOUT);
                var result = await request.Wait();
                if(result == SimpleHttpClient.Request.State.None) {
                    return default;
                }
                if(result == SimpleHttpClient.Request.State.Error) {
                    Debug.LogError(request.GetError());
                    return default;
                }
                var json = await request.Content.ReadAsStringAsync();
                var response = JsonUtility.FromJson<httpbinresponse>(json);

                System.Net.IPAddress.TryParse(response.origin, out var address);
                return address;
            }
            catch(System.Exception e) {
                Debug.LogException(e);
                return default;
            }
        }

        [System.Serializable]
        private class httpbinresponse {
            public string origin = "";
        }

        #endregion
    }
}
