#[repr(u8)]
pub enum Register
{
    AL_EAX_ES,
    CL_ECX_CS,
    DL_EDX_SS,
    BL_EBX_DS,
    AH_ESP_FS,
    CH_EBP_GS,
    DH_ESI,
    BH_EDI
}