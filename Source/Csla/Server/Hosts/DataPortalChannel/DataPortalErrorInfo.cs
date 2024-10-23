//-----------------------------------------------------------------------
// <copyright file="HttpErrorInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Message containing details about any</summary>
//-----------------------------------------------------------------------

using Csla.Serialization.Mobile;

namespace Csla.Server.Hosts.DataPortalChannel
{
  /// <summary>
  /// Message containing details about any
  /// server-side exception.
  /// </summary>
  [Serializable]
  public class DataPortalErrorInfo : ReadOnlyBase<DataPortalErrorInfo>
  {
    /// <summary>
    /// Type name of the exception object.
    /// </summary>
    public static readonly PropertyInfo<string> ExceptionTypeNameProperty = RegisterProperty<string>(c => c.ExceptionTypeName);

    /// <summary>
    /// Type name of the exception object.
    /// </summary>
    public string ExceptionTypeName
    {
      get { return GetProperty(ExceptionTypeNameProperty); }
      private set { LoadProperty(ExceptionTypeNameProperty, value); }
    }

    /// <summary>
    /// Message from the exception object.
    /// </summary>
    public static readonly PropertyInfo<string> MessageProperty = RegisterProperty<string>(c => c.Message);

    /// <summary>
    /// Message from the exception object.
    /// </summary>
    public string Message
    {
      get { return GetProperty(MessageProperty); }
      private set { LoadProperty(MessageProperty, value); }
    }

    /// <summary>
    /// Stack trace from the exception object.
    /// </summary>
    public static readonly PropertyInfo<string> StackTraceProperty = RegisterProperty<string>(c => c.StackTrace);

    /// <summary>
    /// Stack trace from the exception object.
    /// </summary>
    public string StackTrace
    {
      get { return GetProperty(StackTraceProperty); }
      private set { LoadProperty(StackTraceProperty, value); }
    }

    /// <summary>
    /// Source of the exception object.
    /// </summary>
    public static readonly PropertyInfo<string> SourceProperty = RegisterProperty<string>(c => c.Source);

    /// <summary>
    /// Source of the exception object.
    /// </summary>
    public string Source
    {
      get { return GetProperty(SourceProperty); }
      private set { LoadProperty(SourceProperty, value); }
    }

    /// <summary>
    /// TargetSiteName of the exception object.
    /// </summary>
    public static readonly PropertyInfo<string> TargetSiteNameProperty = RegisterProperty<string>(c => c.TargetSiteName);

    /// <summary>
    /// TargetSiteName of the exception object.
    /// </summary>
    public string TargetSiteName
    {
      get { return GetProperty(TargetSiteNameProperty); }
      private set { LoadProperty(TargetSiteNameProperty, value); }
    }

    /// <summary>
    /// HttpErrorInfo object containing information
    /// about any inner exception of the original
    /// exception.
    /// </summary>
    public static readonly PropertyInfo<DataPortalErrorInfo> InnerErrorProperty = RegisterProperty<DataPortalErrorInfo>(c => c.InnerError);

    /// <summary>
    /// HttpErrorInfo object containing information
    /// about any inner exception of the original
    /// exception.
    /// </summary>
    public DataPortalErrorInfo InnerError
    {
      get { return GetProperty(InnerErrorProperty); }
      private set { LoadProperty(InnerErrorProperty, value); }
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext">Current ApplicationContext</param>
    /// <param name="ex">The Exception to encapsulate.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> or <paramref name="ex"/> is <see langword="null"/>.</exception>
    public DataPortalErrorInfo(ApplicationContext applicationContext, Exception ex)
    {
      if (applicationContext is null)
        throw new ArgumentNullException(nameof(applicationContext));
      if (ex is null)
        throw new ArgumentNullException(nameof(ex));

      ApplicationContext = applicationContext;
      ExceptionTypeName = ex.GetType().FullName!;
      Message = ex.Message;
      StackTrace = ex.StackTrace ?? string.Empty;
      Source = ex.Source ?? string.Empty;
      if (ex.InnerException != null)
        InnerError = ApplicationContext.CreateInstance<DataPortalErrorInfo>(ApplicationContext, ex.InnerException);
    }

    /// <summary>
    /// Creates an empty instance of the type.
    /// </summary>
    [Obsolete(MobileFormatter.DefaultCtorObsoleteMessage, error: true)]
    public DataPortalErrorInfo()
    { }
  }
}