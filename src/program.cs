

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static System.Console;

class Program {
	[STAThread]
	public static void Main() {
		var frm    = new Form();
		var ie     = new WebBrowserEx();
		ie.ScriptErrorsSuppressed = false;

		frm.Load += (s,e) => {
			var path = Path.GetFullPath("index.html");
			ie.Navigate(path);
		};

		ie.Dock= DockStyle.Fill;
		frm.Controls.Add(ie);
		frm.ShowDialog();

	}
}

public class WebBrowserEx : System.Windows.Forms.WebBrowser {
	protected class WebBrowserSiteEx : 
		System.Windows.Forms.WebBrowser.WebBrowserSite, 
		IServiceProvider, 
		IInternetSecurityManager {

			static Guid IID_IInternetSecurityManager = 
				Marshal.GenerateGuidForType(typeof(IInternetSecurityManager));

			WebBrowserEx _webBrowser;

			public WebBrowserSiteEx(WebBrowserEx webBrowser) 
				: base(webBrowser) {
					_webBrowser = webBrowser;
			}

			public int QueryService(
					ref Guid guidService, ref Guid riid, out IntPtr ppvObject) {
				if(guidService == IID_IInternetSecurityManager &&
						riid == IID_IInternetSecurityManager) {
					ppvObject = Marshal.GetComInterfaceForObject(this,
							typeof(IInternetSecurityManager));
					return Constants.S_OK;
				}
				ppvObject = IntPtr.Zero;
				return Constants.E_NOINTERFACE;
			}

			public unsafe int SetSecuritySite(void* pSite) {
				return Constants.INET_E_DEFAULT_ACTION;
			}

			public unsafe int GetSecuritySite(void** ppSite) {
				return Constants.INET_E_DEFAULT_ACTION;
			}

			public unsafe int MapUrlToZone(
					string url, int* pdwZone, int dwFlags) {
				//"Local", "Intranet", "Trusted", "Internet", "Restricted"
				*pdwZone = 0; // <= Local.
				return Constants.S_OK;
			}

			public unsafe int GetSecurityId(
					string url, 
					byte* pbSecurityId, 
					int* pcbSecurityId, 
					int dwReserved) {
				return Constants.INET_E_DEFAULT_ACTION;
			}

			public unsafe int ProcessUrlAction(
					string url, int dwAction, byte* pPolicy, int cbPolicy,
					byte* pContext, int cbContext, int dwFlags, 
					int dwReserved) {
				
				WriteLine("Process URL Action");	
				*((int*)pPolicy) = (int)Constants.UrlPolicy.URLPOLICY_ALLOW;
				return Constants.S_OK;
			}

			public unsafe int QueryCustomPolicy(
					string pwszUrl, void* guidKey, byte** ppPolicy, 
					int* pcbPolicy, byte* pContext, int cbContext, 
					int dwReserved) {
				return Constants.INET_E_DEFAULT_ACTION;
			}

			public int SetZoneMapping(
					int dwZone, string lpszPattern, int dwFlags) {
				return Constants.INET_E_DEFAULT_ACTION;
			}

			public unsafe int GetZoneMappings(
					int dwZone, void** ppenumString, int dwFlags) {
				return Constants.INET_E_DEFAULT_ACTION;
			}

		}

	WebBrowserSiteEx _site;

	public WebBrowserEx() {
	}

	protected override WebBrowserSiteBase CreateWebBrowserSiteBase() {
		if(_site == null)
			_site = new WebBrowserSiteEx(this);
		return _site;
	}

}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
public interface IServiceProvider {
	[PreserveSig]
	int QueryService(ref Guid guidService, ref Guid riid, out IntPtr ppvObject);
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("79eac9ee-baf9-11ce-8c82-00aa004ba90b")]
public interface IInternetSecurityManager {
	[PreserveSig]
	unsafe int SetSecuritySite(void* pSite);
	
	[PreserveSig]
	unsafe int GetSecuritySite(void** ppSite);
	
	[PreserveSig]
	unsafe int MapUrlToZone(
			[In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl, 
			int* pdwZone, 
			[In] int dwFlags);

	[PreserveSig]
	unsafe int GetSecurityId(
			[In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl, 
			byte* pbSecurityId, 
			int* pcbSecurityId, 
			int dwReserved);

	[PreserveSig]
	unsafe int ProcessUrlAction(
			[In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl, 
			int dwAction, 
			byte* pPolicy, 
			int cbPolicy, 
			byte* pContext, 
			int cbContext, 
			int dwFlags, 
			int dwReserved);

	[PreserveSig]
	unsafe int QueryCustomPolicy(
			[In, MarshalAs(UnmanagedType.LPWStr)] string pwszUrl, 
			void* guidKey, 
			byte** ppPolicy, 
			int* pcbPolicy, 
			byte* pContext, 
			int cbContext, 
			int dwReserved);

	[PreserveSig]
	int SetZoneMapping(
			int dwZone, 
			[In, MarshalAs(UnmanagedType.LPWStr)] string lpszPattern, 
			int dwFlags);
	
	[PreserveSig]
	unsafe int GetZoneMappings(int dwZone, void** ppenumString, int dwFlags);
}

public static class Constants {
	public const int S_OK = 0;
	public const int E_NOINTERFACE = unchecked((int)0x80004002);
	public const int INET_E_DEFAULT_ACTION = unchecked((int)0x800C0011);
	public enum UrlPolicy {
		URLPOLICY_ALLOW = 0x00,
		URLPOLICY_QUERY = 1,
		URLPOLICY_DISALLOW = 3,
	}
}
