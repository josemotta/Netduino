﻿#pragma checksum "..\..\..\Pages\PageSerialSettings.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2371400135F7AB91E822960227C319D7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Cet.IO.DemoModbusNetduino {
    
    
    /// <summary>
    /// PageSerialSettings
    /// </summary>
    public partial class PageSerialSettings : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 62 "..\..\..\Pages\PageSerialSettings.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox PART_CboSerialPort;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\..\Pages\PageSerialSettings.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock VE_CboSerialPort;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\..\Pages\PageSerialSettings.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PART_TxtSerialSettings;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\..\Pages\PageSerialSettings.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock VE_TxtSerialSettings;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\..\Pages\PageSerialSettings.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PART_TxtAddress;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\..\Pages\PageSerialSettings.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock VE_TxtAddress;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Cet.IO.DemoModbusNetduino_NET45;component/pages/pageserialsettings.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Pages\PageSerialSettings.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 50 "..\..\..\Pages\PageSerialSettings.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonPortRefresh);
            
            #line default
            #line hidden
            return;
            case 2:
            this.PART_CboSerialPort = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.VE_CboSerialPort = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.PART_TxtSerialSettings = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.VE_TxtSerialSettings = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.PART_TxtAddress = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.VE_TxtAddress = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            
            #line 118 "..\..\..\Pages\PageSerialSettings.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ButtonGoNext);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

