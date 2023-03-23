using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using VoiceTexterBot.Configuration;
using VoiceTexterBot.Controller;
using VoiceTexterBot.Services;

namespace VoiceTexterBot
{
    public class Program
	{
		public static async Task Main()
		{
			Console.OutputEncoding = Encoding.Unicode;

			// Объект, отвечающий за постоянный жизненный цикл приложения
			var host = new HostBuilder()
				.ConfigureServices((hostContext, services) => ConfigureServices(services)) // Задаем конфигурацию
				.UseConsoleLifetime() // Позволяет поддерживать приложение активным в консоли
				.Build(); // Собираем

			Console.WriteLine("Сервис запущен");
			// Запускаем сервис
			await host.RunAsync();
			Console.WriteLine("Сервис остановлен");
		}

		static void ConfigureServices(IServiceCollection services)
		{
			AppSettings appSettings = BuildAppSettings();
			services.AddSingleton(appSettings);

			services.AddSingleton<IStorage, MemoryStorage>();
			services.AddTransient<IFileHandler, AudioFileHandler>();
			
			// Подключаем контроллеры сообщений и кнопок
			services.AddTransient<DefaultMessageController>();
			services.AddTransient<VoiceMessageController>();
			services.AddTransient<TextMessageController>();
			services.AddTransient<InlineKeyboardController>();
			
			// Регистрируем объект TelegramBotClient c токеном подключения
			services.AddSingleton<ITelegramBotClient>(provider => new TelegramBotClient(appSettings.BotToken));
			// Регистрируем постоянно активный сервис бота
			services.AddHostedService<Bot>();
		}

		static AppSettings BuildAppSettings()
		{
			return new AppSettings()
			{
				DownloadsFolder = "C:\\Users\\Lenovo\\Downloads",
				BotToken = "6295387110:AAFuNPQnQ7SzhwZIDZew5V0OXZosOK0ocn8",
				AudioFileName = "audio",
				InputAudioFormat = "ogg",
				OutputAudioFormat = "wav",
                InputAudioBitrate = 48000,
            };
		}
	}
}
