////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation.  All rights reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////#include <tinyhal.h>

#include <tinyhal.h>
#include "net_decl_lwip.h"
#include "lwip\netif.h"
#include "lwip\tcp.h"
#include "lwip\tcpip.h"
#include "lwip\dhcp.h"
#include "lwip\pbuf.h"
#include "netif\etharp.h"

#include "AT91_EMAC_LWIP_adapter.h"
#include "AT91_EMAC_lwip.h"
#include "dm9161_lwip.h"

#if defined(ADS_LINKER_BUG__NOT_ALL_UNUSED_VARIABLES_ARE_REMOVED)
#pragma arm section zidata = "g_AT91_EMAC_LWIP_Driver"
#endif

static AT91_EMAC_LWIP_Driver g_AT91_EMAC_LWIP_Driver;
static struct netif          g_AT91_EMAC_NetIF;

HAL_CONTINUATION    InterruptTaskContinuation;
HAL_COMPLETION      LwipUpTimeCompletion;
static BOOL         LwipNetworkStatus = 0;
static UINT32       LwipLastIpAddress = 0;

#if defined(ADS_LINKER_BUG__NOT_ALL_UNUSED_VARIABLES_ARE_REMOVED)
#pragma arm section zidata
#endif

extern AT91_EMAC_LWIP_DRIVER_CONFIG g_AT91_EMAC_LWIP_Config;
extern NETWORK_CONFIG               g_NetworkConfig;

// -- //

void AT91_EMAC__status_callback(struct netif *netif)
{
    if(LwipLastIpAddress != netif->ip_addr.addr)
    {
        Network_PostEvent( NETWORK_EVENT_TYPE_ADDRESS_CHANGED, 0 );
        LwipLastIpAddress = netif->ip_addr.addr;
    }

#if defined(_DEBUG)
    lcd_printf("\f\n\n\n\n\n\nLink Update: \n");
    lcd_printf("         IP: %d.%d.%d.%d\n", (netif->ip_addr.addr >>  0) & 0xFF, 
                                             (netif->ip_addr.addr >>  8) & 0xFF,
                                             (netif->ip_addr.addr >> 16) & 0xFF,
                                             (netif->ip_addr.addr >> 24) & 0xFF);
    lcd_printf("         GW: %d.%d.%d.%d\n", (netif->gw.addr >>  0) & 0xFF, 
                                             (netif->gw.addr >>  8) & 0xFF,
                                             (netif->gw.addr >> 16) & 0xFF,
                                             (netif->gw.addr >> 24) & 0xFF);
    debug_printf("\nLink Update: \r\n");
    debug_printf("         IP: %d.%d.%d.%d\r\n", (netif->ip_addr.addr >>  0) & 0xFF, 
                                             (netif->ip_addr.addr >>  8) & 0xFF,
                                             (netif->ip_addr.addr >> 16) & 0xFF,
                                             (netif->ip_addr.addr >> 24) & 0xFF);
    debug_printf("         GW: %d.%d.%d.%d\r\n", (netif->gw.addr >>  0) & 0xFF, 
                                             (netif->gw.addr >>  8) & 0xFF,
                                             (netif->gw.addr >> 16) & 0xFF,
                                             (netif->gw.addr >> 24) & 0xFF);
#endif
}


err_t AT91_EMAC_ethhw_init(struct netif *myNetIf) 
{ 
    myNetIf->mtu = AT91_EMAC_MAX_FRAME_SIZE;

    /* ethhw_init() is user-defined */
    /* use ip_input instead of ethernet_input for non-ethernet hardware */
    /* (this function is assigned to netif.input and should be called by the hardware driver) */

    /* Assign the xmit routine to the stack's netif and call the driver's Open */

    myNetIf->output = etharp_output;
    myNetIf->linkoutput = AT91_EMAC_LWIP_xmit;
    myNetIf->status_callback = AT91_EMAC__status_callback;

    AT91_EMAC_LWIP_open( myNetIf );

    return 0; 
}

void lwip_interrupt_continuation( void )
{
    NATIVE_PROFILE_PAL_NETWORK();
    GLOBAL_LOCK(irq);
    
    if(!InterruptTaskContinuation.IsLinked())
    {
        InterruptTaskContinuation.Enqueue();
    }
}

void lwip_network_uptime_completion(void *arg)
{
    NATIVE_PROFILE_PAL_NETWORK();

    BOOL status = dm9161_lwip_GetLinkStatus( );

    if(status != LwipNetworkStatus)
    {
        struct netif* pNetIf = (struct netif*)arg;

        if(status)
        {
            SOCK_NetworkConfiguration *pNetCfg = &g_NetworkConfig.NetworkInterfaces[0];

            dm9161_lwip_AutoNegotiate( );
            netif_set_up( pNetIf );

            if(pNetCfg->flags & SOCK_NETWORKCONFIGURATION_FLAGS_DHCP)
            {
                dhcp_start( pNetIf );
            }
            Network_PostEvent( NETWORK_EVENT_TYPE__AVAILABILITY_CHANGED, NETWORK_EVENT_FLAGS_IS_AVAILABLE );
        }
        else
        {
            netif_set_down( (struct netif*)arg );
            Network_PostEvent( NETWORK_EVENT_TYPE__AVAILABILITY_CHANGED, 0);
        }

        LwipNetworkStatus = status;
    }

    LwipUpTimeCompletion.EnqueueDelta64( 2000000 );
}

void InitContinuations( struct netif *pNetIf )
{
    InterruptTaskContinuation.InitializeCallback( (HAL_CALLBACK_FPN)AT91_EMAC_LWIP_recv, &g_AT91_EMAC_NetIF );

    LwipUpTimeCompletion.InitializeForUserMode( (HAL_CALLBACK_FPN)lwip_network_uptime_completion, pNetIf );
    LwipUpTimeCompletion.EnqueueDelta64( 2000000 );
}

BOOL Network_Interface_Bind(int index)
{
    return AT91_EMAC_LWIP_Driver::Bind();
}
int  Network_Interface_Open(int index)
{
    return AT91_EMAC_LWIP_Driver::Open(index);
}
BOOL Network_Interface_Close(int index)
{
    return AT91_EMAC_LWIP_Driver::Close();
}

int AT91_EMAC_LWIP_Driver::Open(int index)
{
    /* Network interface variables */
    struct ip_addr ipaddr, subnetmask, gateway;
    struct netif *pNetIF;
    int len;
    const SOCK_NetworkConfiguration *iface;

    /* Apply network configuration */
    iface = &g_NetworkConfig.NetworkInterfaces[index];

    len = g_AT91_EMAC_NetIF.hwaddr_len;
    if(len == 0 || iface->macAddressLen < len)
    {
        len = iface->macAddressLen;
        g_AT91_EMAC_NetIF.hwaddr_len = len;
    }
    memcpy(g_AT91_EMAC_NetIF.hwaddr, iface->macAddressBuffer, len);

    ipaddr.addr = iface->ipaddr;
    gateway.addr = iface->gateway;
    subnetmask.addr = iface->subnetmask;

    // PHY Power Up
    CPU_GPIO_EnableOutputPin(g_AT91_EMAC_LWIP_Config.PHY_PD_GPIO_Pin, FALSE);

    // Enable Interrupt
    CPU_INTC_ActivateInterrupt(AT91C_ID_EMAC, (HAL_CALLBACK_FPN)AT91_EMAC_LWIP_interrupt, &g_AT91_EMAC_NetIF);

    /* Initialize the continuation routine for the driver interrupt and receive */    
    InitContinuations( pNetIF );

    pNetIF = netif_add( &g_AT91_EMAC_NetIF, &ipaddr, &subnetmask, &gateway, NULL, AT91_EMAC_ethhw_init, ethernet_input );

    netif_set_default( pNetIF );

    LwipNetworkStatus = dm9161_lwip_GetLinkStatus( );
    if (LwipNetworkStatus)
    {
        netif_set_up( pNetIF );
    }

    return g_AT91_EMAC_NetIF.num;
}

// -- //

BOOL AT91_EMAC_LWIP_Driver::Close(void)
{
    LwipUpTimeCompletion.Abort();

    CPU_INTC_DeactivateInterrupt(AT91C_ID_EMAC);

    InterruptTaskContinuation.Abort();

    LwipNetworkStatus = 0;

    netif_set_down( &g_AT91_EMAC_NetIF );
    netif_remove( &g_AT91_EMAC_NetIF );

    AT91_EMAC_LWIP_close( &g_AT91_EMAC_NetIF );
    
    memset( &g_AT91_EMAC_NetIF, 0, sizeof g_AT91_EMAC_NetIF );

    return TRUE;
}

BOOL AT91_EMAC_LWIP_Driver::Bind(void)
{
    return TRUE;
}

