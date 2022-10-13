using PluginUpdater.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PluginUpdater.Models
{
    public class ProgressInfo : BaseNotifyPropertyChanged
    {
        public enum TypeAction { Install, Delete, Update }

        private IPlugin m_plagin;
        private int m_value;
        private TypeAction m_typeAction;

        public IPlugin Plagin => m_plagin;
        public int Value => m_value;
        public TypeAction ActionType => m_typeAction;

        private bool m_isComleted;
        public bool IsComleted {
            get { return m_isComleted; }
            set 
            { 
                m_isComleted = value;
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(IsComleted));
            }
        }

        public string Info {
            get
            {
                string title = $"{ m_plagin.ID }";
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
        public string Status => IsComleted ? "Завершено" : "Выполняется...";
        public string Path => m_plagin.Path;

        public ProgressInfo(IPlugin plagin, int value, TypeAction typeAction, bool isCompleted = false)
        {
            m_plagin = plagin;
            m_value = value;
            m_typeAction = typeAction;
            m_isComleted = isCompleted;
        }

        public void Copy(ProgressInfo other)
        {
            m_value = other.Value;
            m_typeAction = other.m_typeAction;
            IsComleted = other.IsComleted;
        }
    }
}
