# KonturTest

WPF-приложение для конвертации бинарных файлов в CSV.

## Задачи

**Задача 1 — Пакеты → CSV**
Читает бинарный файл пакетов (1456 байт/пакет): заголовок 16 байт + 60 блоков по 6 каналов (int32). Вычисляет среднее по каждому каналу и записывает в CSV.

**Задача 2 — Битовые поля → CSV**
Читает бинарный файл 32-битных слов, разбирает битовые поля и записывает каждое слово строкой в CSV.

## Структура

```
Infrastructure/   ViewModelBase, RelayCommand, ProcessingViewModelBase
Models/           PacketRecord, BitFieldRecord, AppSettings
Services/         Task1ProcessingService, Task2ProcessingService, SettingsService
ViewModels/       MainViewModel, Task1ViewModel, Task2ViewModel
Views/            Task1View.xaml, Task2View.xaml
```

Пути к файлам сохраняются в `%AppData%\KonturTest\settings.json`.
