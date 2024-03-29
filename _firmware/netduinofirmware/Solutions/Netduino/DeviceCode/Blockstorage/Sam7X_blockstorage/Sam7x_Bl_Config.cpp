////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation.  All rights reserved.
// Portions Copyright (c) Secret Labs LLC.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#include <tinyhal.h>

#include <Targets\Native\AT91\DeviceCode\AT91_BL_EFC\AT91_BL_EFC.h>




// ****** TO BE MODIFIED *****************************
// BlockDeviceInformation


#define FLASH_MANUFACTURER_CODE                 0xffff
#define FLASH_DEVICE_CODE                       0xffff
#define FLASH_BLOCK_COUNT                       64


#define SAM7X_BLOCK_ERASE_TYPICAL_TIME_USEC     96000
#define SAM7X_SECTOR_WRITE_TYPICAL_TIME_USEC    48000
#define SAM7X_BLOCK_ERASE_MAX_TIME_USEC         96000
#define SAM7X_SECTOR_WRITE_MAX_TIME_USEC        48000


#define SAM7X_BS__BASE_ADDRESS                  FLASH_MEMORY_Base
#define SAM7X_BS__SIZE_IN_BYTES                 0x80000

//dummy
#define SAM7X_BS__CHIP_SELECT                   0
#define SAM7X_BS__WAIT_STATES                   0
#define SAM7X_BS__RELEASE_COUNTS                0
#define SAM7X_BS__BIT_WIDTH                     16
#define SAM7X_BS__WP_GPIO_PIN                   GPIO_PIN_NONE
#define SAM7X_BS__WP_ACTIVE                     FALSE

#define SAM7X_BS__IS_REMOVABLE                  FALSE
#define SAM7X_BS__SUPPORTS_XIP                  TRUE
#define SAM7X_BS__WRITE_PROTECTED               FALSE

#define SAM7X_BS__NUM_REGIONS                   1

#define BYTES_PER_SECTOR                        4
    
#define REGION1_XIP_ADDRESS                     SAM7X_BS__BASE_ADDRESS
#define REGION1_BYTES_PER_BLK                   0x2000
#define REGION1_NUM_OF_BLOCKS                   FLASH_BLOCK_COUNT

//*******************************************************//



//--//


#if defined(MEMORY_BLOCKTYPE_GCC_SPECIAL_BOOTSTRAP)
#undef MEMORY_BLOCKTYPE_GCC_SPECIAL_BOOTSTRAP 
#endif

#if defined(MEMORY_BLOCKTYPE_GCC_SPECIAL_CODE)
#undef MEMORY_BLOCKTYPE_GCC_SPECIAL_CODE
#endif

#ifdef __GNUC__
#define MEMORY_BLOCKTYPE_GCC_SPECIAL_BOOTSTRAP  BlockRange::BLOCKTYPE_BOOTSTRAP
#define MEMORY_BLOCKTYPE_GCC_SPECIAL_CODE       BlockRange::BLOCKTYPE_CODE
#else
#define MEMORY_BLOCKTYPE_GCC_SPECIAL_BOOTSTRAP  BlockRange::BLOCKTYPE_CODE
#define MEMORY_BLOCKTYPE_GCC_SPECIAL_CODE       BlockRange::BLOCKTYPE_DEPLOYMENT
#endif

//--//


const BlockRange g_SAM7X_BS_BlockRange[] =
{
    //
    { BlockRange::BLOCKTYPE_BOOTSTRAP       ,  0,  5 },
    { MEMORY_BLOCKTYPE_GCC_SPECIAL_BOOTSTRAP,  6, 14 },
    { BlockRange::BLOCKTYPE_CODE            , 15, 44 },
    { MEMORY_BLOCKTYPE_GCC_SPECIAL_CODE     , 45, 46 },
    { BlockRange::BLOCKTYPE_DEPLOYMENT      , 47, 60 },
    { BlockRange::BLOCKTYPE_STORAGE_A       , 61, 61 },
    { BlockRange::BLOCKTYPE_STORAGE_B       , 62, 62 },
    { BlockRange::BLOCKTYPE_CONFIG          , 63, 63 }
};

//--//

#undef MEMORY_BLOCKTYPE_GCC_SPECIAL_BOOTSTRAP 
#undef MEMORY_BLOCKTYPE_GCC_SPECIAL_CODE    

//--//

#if defined(ADS_LINKER_BUG__NOT_ALL_UNUSED_VARIABLES_ARE_REMOVED)
#pragma arm section rwdata = "g_SAM7X_BS_Config"
#endif




const BlockRegionInfo  g_SAM7X_BS_BlkRegion[SAM7X_BS__NUM_REGIONS] = 
{
    
    REGION1_XIP_ADDRESS,                // ByteAddress     Address;            // Start address
    REGION1_NUM_OF_BLOCKS,              // UINT32          NumBlocks;          // total number of blocks in this region
    REGION1_BYTES_PER_BLK,              // UINT32          BytesPerBlock;      // Total number of bytes per block (MUST be SectorsPerBlock * DataBytesPerSector)

    ARRAYSIZE_CONST_EXPR(g_SAM7X_BS_BlockRange),
    g_SAM7X_BS_BlockRange,
};

//--//


BlockDeviceInfo g_SAM7X_BS_DeviceInfo=
{
    {
        SAM7X_BS__IS_REMOVABLE,                     // BOOL Removable;
        SAM7X_BS__SUPPORTS_XIP,                     // BOOL SupportsXIP;
        SAM7X_BS__WRITE_PROTECTED,                  // BOOL WriteProtected
    },
    
    SAM7X_SECTOR_WRITE_MAX_TIME_USEC,               // UINT32 Duration_Max_WordWrite_uSec;
    SAM7X_BLOCK_ERASE_MAX_TIME_USEC,                // UINT32 Duration_Max_SectorErase_uSec;
    BYTES_PER_SECTOR,                               // BytesPerSector; // Bytes Per Sector

    FLASH_MEMORY_Size,                              // UINT32 Size;

    SAM7X_BS__NUM_REGIONS,                          // UINT32 NumRegions;
    g_SAM7X_BS_BlkRegion,                           // const BlockRegionInfo* pRegions;
};


struct MEMORY_MAPPED_NOR_BLOCK_CONFIG g_SAM7X_BS_Config =
{
    { // BLOCK_CONFIG
        {
            SAM7X_BS__WP_GPIO_PIN,                  // GPIO_PIN             Pin;
            SAM7X_BS__WP_ACTIVE,                    // BOOL                 ActiveState;
        },

        &g_SAM7X_BS_DeviceInfo,                     // BlockDeviceinfo
    },
    
    { // CPU_MEMORY_CONFIG
        SAM7X_BS__CHIP_SELECT,                      // UINT8                CPU_MEMORY_CONFIG::ChipSelect;
        TRUE,                                       // UINT8                CPU_MEMORY_CONFIG::ReadOnly;
        SAM7X_BS__WAIT_STATES,                      // UINT32               CPU_MEMORY_CONFIG::WaitStates;
        SAM7X_BS__RELEASE_COUNTS,                   // UINT32               CPU_MEMORY_CONFIG::ReleaseCounts;
        SAM7X_BS__BIT_WIDTH,                        // UINT32               CPU_MEMORY_CONFIG::BitWidth;
        SAM7X_BS__BASE_ADDRESS,                     // UINT32               CPU_MEMORY_CONFIG::BaseAddress;
        SAM7X_BS__SIZE_IN_BYTES,                    // UINT32               CPU_MEMORY_CONFIG::SizeInBytes;
        0,                                          // UINT8                CPU_MEMORY_CONFIG::XREADYEnable 
        0,                                          // UINT8                CPU_MEMORY_CONFIG::ByteSignalsForRead 
        0,                                          // UINT8                CPU_MEMORY_CONFIG::ExternalBufferEnable
    },

    0x1055AADD,                                     // UINT32               ChipProtection;
    FLASH_MANUFACTURER_CODE,                        // FLASH_WORD           ManufacturerCode;
    FLASH_DEVICE_CODE,                              // FLASH_WORD           DeviceCode;
};


//--//

#if defined(ADS_LINKER_BUG__NOT_ALL_UNUSED_VARIABLES_ARE_REMOVED)
#pragma arm section rodata = "g_SAM7X_BS"
#endif

struct BlockStorageDevice g_SAM7X_BS;

#if defined(ADS_LINKER_BUG__NOT_ALL_UNUSED_VARIABLES_ARE_REMOVED)
#pragma arm section rodata 
#endif 

//--//

