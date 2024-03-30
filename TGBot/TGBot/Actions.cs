using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Net;
using System.Xml;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TGBot
{
    public static class Actions
    {
        public static async Task Start(ITelegramBotClient botClient, long chatId, ReplyKeyboardMarkup keyboardMarkup, CancellationToken token)
        {
            await botClient.SendTextMessageAsync(chatId, text: "Привіт😊, я бот який дає вам випадковий аніме тайтл❗", cancellationToken: token, replyMarkup: keyboardMarkup);
        }
        public static async Task RandAnimes(ITelegramBotClient botClient, long chatId, CancellationToken token)
        {
            bool isfind = false;
            string title = String.Empty;
            string year = String.Empty;
            string alttitle = String.Empty;
            string description = String.Empty;
            string urlimage = String.Empty;
            string web = String.Empty;
            var waitMessage = await botClient.SendTextMessageAsync(chatId, text: "Please wait...", cancellationToken: token);
            do
            {
                Random random = new Random();
                string xmlRequest = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                                <Request>
                                    <Data>
                                        <Name>Anime title</Name>
                                    </Data>
                                </Request>";
                string url = "https://cdn.animenewsnetwork.com/encyclopedia/api.xml?anime=" + random.Next(0, 30000);
                Console.WriteLine(url);
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "text/xml";
                    byte[] xmlBytes = Encoding.UTF8.GetBytes(xmlRequest);
                    request.ContentLength = xmlBytes.Length;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(xmlBytes, 0, xmlBytes.Length);
                    }
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                        {
                            string responseXml = streamReader.ReadToEnd();
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(responseXml);
                            XmlNodeList genresTitleNodes = xmlDoc.SelectNodes("//info[@type='Alternative title']");
                            foreach (XmlNode genresTitleNode in genresTitleNodes)
                            {
                                if (genresTitleNode != null && genresTitleNode.InnerText == "erotica")
                                    throw new Exception();
                            }
                            XmlNode mainTitleNode = xmlDoc.SelectSingleNode("//info[@type='Main title']");
                            if (mainTitleNode != null)
                                title = mainTitleNode.InnerText;
                            else
                                throw new Exception();
                            XmlNode VintageNode = xmlDoc.SelectSingleNode("//info[@type='Vintage']");
                            if (VintageNode != null)
                                year = VintageNode.InnerText.Substring(0, 4);
                            XmlNodeList altTitleNodes = xmlDoc.SelectNodes("//info[@type='Alternative title']");
                            if (altTitleNodes != null)
                            {
                                alttitle = "\n\nАльтернативні назви:\n";
                                foreach (XmlNode altTitleNode in altTitleNodes)
                                {
                                    string lang = altTitleNode.Attributes["lang"]?.Value;
                                    if (lang == "EN" || lang == "JA")
                                        alttitle += $"{altTitleNode.InnerText}\n";
                                }
                                alttitle += "\n";
                            }
                            XmlNode imageUrlNode = xmlDoc.SelectSingleNode("//info[@type='Picture']/@src");
                            if (imageUrlNode != null)
                                urlimage = imageUrlNode.InnerText;
                            else
                                throw new Exception();
                            XmlNode descriptionUrlNode = xmlDoc.SelectSingleNode("//info[@type='Plot Summary']");
                            if (descriptionUrlNode != null)
                            {
                                description = $"\n{descriptionUrlNode.InnerText}\n";
                            }
                            else
                                throw new Exception();
                            XmlNode webUrlNode = xmlDoc.SelectSingleNode("//info[@type='Official website']/@href");
                            if (webUrlNode != null)
                            {
                                web = webUrlNode.InnerText;
                            }
                            else
                                web = null;
                            isfind = true;
                            Console.WriteLine($"Finding: {url}");
                        }
                    }
                }
                catch (Exception)
                {
                    isfind = false;
                }
            } while (!isfind);
            if (web != null)
            {
                await botClient.DeleteMessageAsync(chatId, waitMessage.MessageId, cancellationToken: token);
                await botClient.SendPhotoAsync(chatId, photo: InputFile.FromUri(urlimage), caption: $"<b>{title}</b>, <i>{year}</i>{alttitle}<b>Опис:</b><i>{description}</i>\n<b>Офіційна сторінка:</b> \n{web}", parseMode: ParseMode.Html);
            }
            else
            {
                await botClient.DeleteMessageAsync(chatId, waitMessage.MessageId, cancellationToken: token);
                await botClient.SendPhotoAsync(chatId, photo: InputFile.FromUri(urlimage), caption: $"<b>{title}</b>, <i>{year}</i>{alttitle}<b>Опис:</b><i>{description}</i>", parseMode: ParseMode.Html);
            }
            }
        public static async Task About(ITelegramBotClient botClient, long chatId, ReplyKeyboardMarkup keyboardMarkup, CancellationToken token)
        {
            await botClient.SendTextMessageAsync(chatId, text: "🤖Bot🤖 created during second practice by 'АППЗ .Net'❗\n\n🤔API which used: https://cdn.animenewsnetwork.com/encyclopedia/api.xml", cancellationToken: token, replyMarkup: keyboardMarkup, parseMode: ParseMode.Html);
        }
    }
}
