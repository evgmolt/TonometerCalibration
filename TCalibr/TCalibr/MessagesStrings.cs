using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCalibr
{
    internal static class MessagesStrings
    {
        public static string NoConnection = "Тонометр не подключен";
        public static string Connect = "Подключите тонометр (в сервисном режиме)";
        public static string SetPressure = "Установите значение давления";
        public static string Step01 = "100 мм.рт.ст";
        public static string Step02 = "200 мм.рт.ст";
        public static string Step03 = "250 мм.рт.ст";
        //public static string Step01 = "0,015 МПа";
        //public static string Step02 = "0,025 МПа";
        //public static string Step03 = "0,035 МПа";
        public static string PressContinue = "Нажмите кнопку [Продолжить]";
        public static string PressWrite = "Нажмите кнопку [Записать]";
        public static string Completed = "Калибровка завершена. Отключите тонометр";
        public static string CloseValve = "Откройте клапан";
    }
}
