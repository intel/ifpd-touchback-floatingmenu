#pragma once
#include <stdint.h>

const uint8_t *hid_touch_configuration_descriptor_fs(void);

void hid_touch_send(uint16_t x, uint16_t y,
                    uint8_t contact_id, uint8_t tip_switch,
                    uint8_t pressure);