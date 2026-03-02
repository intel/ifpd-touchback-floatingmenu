#include "hid_touch.h"
#include "tusb.h"
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"

#define REPORT_ID_TOUCH         1
#define REPORT_ID_MAX_COUNT     2
#define ITF_NUM_HID             0
#define EPNUM_HID               0x81
#define HID_POLL_INTERVAL_MS    1
#define MAX_CONTACTS            1
#define TOUCH_MAX_X             32767
#define TOUCH_MAX_Y             32767
#define CONFIG_TOTAL_LEN        (TUD_CONFIG_DESC_LEN + TUD_HID_DESC_LEN)

// =================== REPORT DESCRIPTOR ===================

const uint8_t hid_report_descriptor[] = {
    // --- Touch Input Report (ID 1) ---
    0x05, 0x0D,                     // Usage Page (Digitizers)
    0x09, 0x04,                     // Usage (Touch Screen)
    0xA1, 0x01,                     // Collection (Application)

      0x85, REPORT_ID_TOUCH,        //   Report ID (1)

      0x09, 0x22,                   //   Usage (Finger)
      0xA1, 0x02,                   //   Collection (Logical)

        // Tip Switch (1 bit)
        0x09, 0x42,                 //     Usage (Tip Switch)
        0x15, 0x00,                 //     Logical Min (0)
        0x25, 0x01,                 //     Logical Max (1)
        0x75, 0x01,                 //     Report Size (1)
        0x95, 0x01,                 //     Report Count (1)
        0x81, 0x02,                 //     Input (Data, Var, Abs)

        // Padding (7 bits)
        0x75, 0x07,                 //     Report Size (7)
        0x95, 0x01,                 //     Report Count (1)
        0x81, 0x03,                 //     Input (Const, Var, Abs)

        // Contact ID (8 bits)
        0x09, 0x51,                 //     Usage (Contact Identifier)
        0x15, 0x00,                 //     Logical Min (0)
        0x25, 0x7F,                 //     Logical Max (127)
        0x75, 0x08,                 //     Report Size (8)
        0x95, 0x01,                 //     Report Count (1)
        0x81, 0x02,                 //     Input (Data, Var, Abs)

        // X (16 bits)
        0x05, 0x01,                 //     Usage Page (Generic Desktop)
        0x09, 0x30,                 //     Usage (X)
        0x16, 0x00, 0x00,           //     Logical Min (0)
        0x26, 0xFF, 0x7F,           //     Logical Max (32767)
        0x75, 0x10,                 //     Report Size (16)
        0x95, 0x01,                 //     Report Count (1)
        0x81, 0x02,                 //     Input (Data, Var, Abs)

        // Y (16 bits)
        0x09, 0x31,                 //     Usage (Y)
        0x16, 0x00, 0x00,           //     Logical Min (0)
        0x26, 0xFF, 0x7F,           //     Logical Max (32767)
        0x75, 0x10,                 //     Report Size (16)
        0x95, 0x01,                 //     Report Count (1)
        0x81, 0x02,                 //     Input (Data, Var, Abs)

      0xC0,                         //   End Collection (Logical)

      // Contact Count (8 bits)
      0x05, 0x0D,                   //   Usage Page (Digitizers)
      0x09, 0x54,                   //   Usage (Contact Count)
      0x15, 0x00,                   //   Logical Min (0)
      0x25, MAX_CONTACTS,           //   Logical Max (1)
      0x75, 0x08,                   //   Report Size (8)
      0x95, 0x01,                   //   Report Count (1)
      0x81, 0x02,                   //   Input (Data, Var, Abs)

      // --- Feature Report: Contact Count Maximum (ID 2) ---
      0x85, REPORT_ID_MAX_COUNT,    //   Report ID (2)
      0x09, 0x55,                   //   Usage (Contact Count Maximum)
      0x15, 0x00,                   //   Logical Min (0)
      0x25, MAX_CONTACTS,           //   Logical Max (1)
      0x75, 0x08,                   //   Report Size (8)
      0x95, 0x01,                   //   Report Count (1)
      0xB1, 0x02,                   //   Feature (Data, Var, Abs)

    0xC0                            // End Collection (Application)
};

// =================== CONFIGURATION DESCRIPTOR ===================

static uint8_t const hid_fs_configuration_descriptor[] = {
    TUD_CONFIG_DESCRIPTOR(1, 1, 0, CONFIG_TOTAL_LEN, 0x00, 100),
    TUD_HID_DESCRIPTOR(ITF_NUM_HID, 0, HID_ITF_PROTOCOL_NONE,
                       sizeof(hid_report_descriptor), EPNUM_HID,
                       CFG_TUD_HID_EP_BUFSIZE, HID_POLL_INTERVAL_MS)
};

const uint8_t *hid_touch_configuration_descriptor_fs(void)
{
    return hid_fs_configuration_descriptor;
}

// =================== TINYUSB CALLBACKS ===================

uint8_t const *tud_hid_descriptor_report_cb(uint8_t instance)
{
    (void)instance;
    return hid_report_descriptor;
}

uint16_t tud_hid_get_report_cb(uint8_t instance, uint8_t report_id,
                                hid_report_type_t report_type,
                                uint8_t *buffer, uint16_t reqlen)
{
    // Respond to host request for Contact Count Maximum feature report
    if (report_type == HID_REPORT_TYPE_FEATURE &&
        report_id == REPORT_ID_MAX_COUNT)
    {
        buffer[0] = MAX_CONTACTS;
        return 1;
    }
    return 0;
}

void tud_hid_set_report_cb(uint8_t instance, uint8_t report_id,
                            hid_report_type_t report_type,
                            uint8_t const *buffer, uint16_t bufsize)
{
    (void)instance;
    (void)report_id;
    (void)report_type;
    (void)buffer;
    (void)bufsize;
}

// =================== REPORT STRUCT ===================
// Must match descriptor exactly:
// byte0: tip_switch(1) + padding(7)
// byte1: contact_id(8)
// bytes2-3: x(16 LE)
// bytes4-5: y(16 LE)
// byte6: contact_count(8)
typedef struct {
    uint8_t  flags;          // bit0=TipSwitch, bits1-7=padding
    uint8_t  contact_id;
    uint16_t x;
    uint16_t y;
    uint8_t  contact_count;
} __attribute__((packed)) touch_report_t;

// =================== PUBLIC SEND FUNCTION ===================

void hid_touch_send(uint16_t x, uint16_t y,
                    uint8_t contact_id, uint8_t tip_switch, uint8_t pressure)
{
    // Wait up to 100 ms for HID endpoint to be ready
    for (int i = 0; i < 10; i++) {
        if (tud_hid_ready()) break;
        vTaskDelay(pdMS_TO_TICKS(10));
    }
    if (!tud_hid_ready()) return;

    touch_report_t report;
    report.flags         = tip_switch ? 0x01 : 0x00;  // bit0=TipSwitch only
    report.contact_id    = contact_id;
    report.x             = x;
    report.y             = y;
    report.contact_count = tip_switch ? 1 : 0;

    tud_hid_report(REPORT_ID_TOUCH, &report, sizeof(report));

    // If this is a touch-down (tip=1), send a matching up (tip=0) after 16ms
    // only if no further events are expected. Caller should send tip=0 explicitly.
    // For tip=0 (lift), also send contact_count=0 to inform host.
    if (!tip_switch) {
        vTaskDelay(pdMS_TO_TICKS(8));
        report.flags         = 0x00;
        report.contact_count = 0;
        // Wait for ready again
        for (int i = 0; i < 5; i++) {
            if (tud_hid_ready()) break;
            vTaskDelay(pdMS_TO_TICKS(10));
        }
        tud_hid_report(REPORT_ID_TOUCH, &report, sizeof(report));
    }
}