using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/// <summary>
/// Сводное описание для Leonbets
/// </summary>
  class Leonbets : IDisposable
    {
        private const string USER_AGENT =
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_3) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/45.0.2414.0 Safari/537.36";

        private HttpClientHandler m_Handler;
        private HttpClient m_Client;

        public string UserName { get; private set; }
        public string AccountNumber { get; private set; }
        public string Balance { get; private set; }

        /// <summary>
        /// Клиент. После авторизации можно его использовать для отправки/получения различной информации с сайта
        /// </summary>
        public HttpClient Client
        {
            get { return m_Client; }
        }

        public Leonbets()
        {
            m_Handler = new HttpClientHandler();

            m_Client = new HttpClient(m_Handler);
            m_Client.DefaultRequestHeaders.UserAgent.ParseAdd(USER_AGENT);
            m_Client.BaseAddress = new Uri("http://amr.teleofis.ru/");
        }

        public void Dispose()
        {
            m_Client.Dispose();
            m_Handler.Dispose();
        }

        /// <summary>
        /// Попытка авторизации. 
        /// </summary>
        /// <param name="login">e-mail/id счета</param>
        /// <param name="password">Пароль</param>
        /// <returns>В случае удачной авторизации - true + заполнение информации о пользователе</returns>
        public async Task<bool> TryLogon(string login, string password)
        {
            await DoJavascriptValidation();

            var fields = new Dictionary<string, string>()
                {
                    { "username", login },
                    { "password", password }
                };

            using (var request = new HttpRequestMessage(HttpMethod.Post, "/login"))
            {
                request.Headers.Referrer = m_Client.BaseAddress;
                request.Content = new FormUrlEncodedContent(fields);

                using (var response = await m_Client.SendAsync(request))
                {
                    var config = Configuration.Default.WithCookies();
                    var html = response.Content.ReadAsStringAsync();
                    //
                    
                    
                    // проверяем наличие элементов с информацией о пользователе
                    var document = new HtmlParser();
                    
                    var userInfo = document.Parse(html.ToString()).QuerySelector("#login_form");

                    if (userInfo != null)
                    {
                        //SetUserInfo(userInfo);
                        return true;
                    }
                    else
                    {
                        return false;   // Не авторизовались почему-то..
                    }
                     
                }
            }
        }

        /// <summary>
        /// Заполняем информацию о пользователе, раз уж уже получили её
        /// </summary>
        /// <param name="userInfoElement"></param>
        private void SetUserInfo(IElement userInfoElement)
        {
            this.UserName = userInfoElement.Children[0].TextContent;

            var accountInfo = userInfoElement.Children[1];
            this.AccountNumber = accountInfo.Children[1].TextContent;
            this.Balance = accountInfo.Children[3].TextContent;
        }


        /// <summary>
        /// проверка на JavaScript
        /// </summary>
        private async Task DoJavascriptValidation()
        {
            // Получаем главную страницу, из JS получаем cookie
            var html = await m_Client.GetStringAsync("/");
            var cookie = GetCookieFromScript(html);
            m_Handler.CookieContainer.Add(m_Client.BaseAddress, cookie);

            await m_Client.GetStringAsync("/");
        }

        /// <summary>
        /// Получаем Cookie bp
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private Cookie GetCookieFromScript(string html)
        {
            var match = Regex.Match(html, @"setCookie\('(\w+)', '([\d\.]+)', (\d+)\);");
            var expiredays = int.Parse(match.Groups[3].Value);

            return new Cookie()
            {
                Name = match.Groups[1].Value,
                Value = match.Groups[2].Value,
                Expires = DateTime.Now.AddDays(expiredays)
            };
        }
    }