using PluginUpdater.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PluginUpdater.Models
{
    public enum TypeAction { Install, Delete, Update }
    public enum TypeResult { None, Comleted, Fail, Cancel }

    public interface IProgressInfo
    {
        public IPlugin Plagin { get; }
        public int Value { get; }
        public string Info { get; }
        public string ErrorMessage { get; }

        public string InfoVersion { get; }
        public string Status { get; }
        public string Path { get; }

        public void Copy(IProgressInfo other);
        public TypeAction ActionType { get; }
        public TypeResult StatusResult { get; }
    }

    public class ProgressInfo : BaseNotifyPropertyChanged, IProgressInfo
    {
        private PluginViewModel m_plagin;
        private int m_value;
        private TypeAction m_typeAction;
        private TypeResult m_statusResult;

        public string ErrorMessage => m_plagin.ErrorMessage;
        public IPlugin Plagin => m_plagin.Plugin;
        public int Value => m_value;
        public TypeAction ActionType => m_typeAction;
        public TypeResult StatusResult => m_statusResult;

        private void UpdateStatusView()
        {
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(StatusResult));
        }

        public string Info {
            get
            {
                string title = m_plagin.ID;
                switch (m_typeAction)
                {
                    case TypeAction.Delete:
                        return $"Удаление плагина: {title}";
                    case TypeAction.Install:
                        return $"Установка плагина: {title}";
                    case TypeAction.Update:
                        return $"Обновление плагина: {title}";
                    default:
                        return "...";
                }
            }
        }

        public string InfoVersion => $"версия: {m_plagin.Version}";
        public string Status
        {
            get 
            {
                if (m_statusResult == TypeResult.Fail)
                    return "Прервано";
                return m_statusResult == TypeResult.Comleted ? "Завершено" : "Выполняется...";
            }
        }
        public string Path => m_plagin.Path;

        public ProgressInfo(PluginViewModel plagin, int value, TypeAction typeAction, TypeResult statusResult = TypeResult.None)
        {
            m_plagin = plagin;
            m_value = value;
            m_typeAction = typeAction;
            m_statusResult = statusResult;
        }

        public void Copy(IProgressInfo other)
        {
            m_value = other.Value;
            m_typeAction = other.ActionType;
            m_statusResult = other.StatusResult;
            UpdateStatusView();
        }
    }
}
