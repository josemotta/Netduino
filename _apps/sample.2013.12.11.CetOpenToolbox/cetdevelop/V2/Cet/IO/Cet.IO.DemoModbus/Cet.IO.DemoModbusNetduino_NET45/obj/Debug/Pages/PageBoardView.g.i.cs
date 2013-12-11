﻿#pragma checksum "..\..\..\Pages\PageBoardView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "22F9C1CB8CFEEA97D53F3981FE949135"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Cet.IO.DemoModbusNetduino;
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
using ToggleSwitch;


namespace Cet.IO.DemoModbusNetduino {
    
    
    /// <summary>
    /// PageBoardView
    /// </summary>
    public partial class PageBoardView : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 183 "..\..\..\Pages\PageBoardView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock PART_Settings;
        
        #line default
        #line hidden
        
        
        #line 196 "..\..\..\Pages\PageBoardView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock PART_Port;
        
        #line default
        #line hidden
        
        
        #line 219 "..\..\..\Pages\PageBoardView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid CtrPoller;
        
        #line default
        #line hidden
        
        
        #line 248 "..\..\..\Pages\PageBoardView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal ToggleSwitch.HorizontalToggleSwitch PART_EnablePolling;
        
        #line default
        #line hidden
        
        
        #line 253 "..\..\..\Pages\PageBoardView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Cet.IO.DemoModbusNetduino.LedControl PART_ActLed;
        
        #line default
        #line hidden
        
        
        #line 264 "..\..\..\Pages\PageBoardView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ItemsControl IcAnalogs;
        
        #line default
        #line hidden
        
        
        #line 295 "..\..\..\Pages\PageBoardView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ItemsControl IcUpper;
        
        #line default
        #line hidden
        
        
        #line 315 "..\..\..\Pages\PageBoardView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ItemsControl IcLower;
        
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
            System.Uri resourceLocater = new System.Uri("/Cet.IO.DemoModbusNetduino_NET45;component/pages/pageboardview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Pages\PageBoardView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            this.PART_Settings = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.PART_Port = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.CtrPoller = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.PART_EnablePolling = ((ToggleSwitch.HorizontalToggleSwitch)(target));
            return;
            case 5:
            this.PART_ActLed = ((Cet.IO.DemoModbusNetduino.LedControl)(target));
            return;
            case 6:
            this.IcAnalogs = ((System.Windows.Controls.ItemsControl)(target));
            return;
            case 7:
            this.IcUpper = ((System.Windows.Controls.ItemsControl)(target));
            return;
            case 8:
            this.IcLower = ((System.Windows.Controls.ItemsControl)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

