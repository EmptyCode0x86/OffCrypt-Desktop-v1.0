# OffCryptDesktop User Manual

**Version:** 2.0  
**Last Updated:** January 2025  
**Cryptographic Standards:** NIST 2024 Compliant  

---

## Table of Contents

1. [Getting Started](#1-getting-started)
2. [Password-Based Encryption](#2-password-based-encryption)
3. [X25519 Public Key Encryption](#3-x25519-public-key-encryption)
4. [Advanced Settings](#4-advanced-settings)
5. [Identity & Contact Management](#5-identity--contact-management)
6. [Disappearing Messages](#6-disappearing-messages)
7. [Security Best Practices](#7-security-best-practices)
8. [File Operations](#8-file-operations)
9. [Troubleshooting](#9-troubleshooting)
10. [Technical Reference](#10-technical-reference)

---

## 1. Getting Started

### First Launch Setup

When you launch OffCryptDesktop for the first time:

1. **Welcome Screen**: The application will display a modern dark theme interface
2. **Default Settings**: Configuration files will be automatically created in your user profile
3. **Three Main Tabs**: You'll see three primary tabs:
   - **Tab 1**: Password-Based Encryption
   - **Tab 2**: X25519 Public Key Encryption  
   - **Tab 3**: Advanced Settings

### Initial Password Setup

**⚠️ SECURITY CRITICAL**: Set a strong master password immediately.

1. Navigate to **Tab 1** (Password-Based Encryption)
2. Locate the **"Set Password"** section in the top-left
3. Enter a password in the text field (minimum 8 characters)
4. Click **"Set Password"** button
5. **Success**: You'll see "Password set successfully!" confirmation

**Password Requirements:**
- Minimum 8 characters
- Mix of uppercase, lowercase, numbers recommended
- Avoid dictionary words or personal information
- Password is stored securely in memory as char[] array

### Interface Overview

#### Tab 1: Password-Based Encryption
- **Password Section**: Set/change master password
- **Message Input**: Large text area for messages
- **Disappearing Messages**: Checkboxes and settings for timed deletion
- **Encrypt/Decrypt**: Main action buttons
- **File Operations**: Create/import encrypted files

#### Tab 2: X25519 Public Key Encryption
- **Key Management**: Generate and manage X25519 key pairs
- **Public Key Display**: Shows your public key for sharing
- **Recipient Management**: Import recipient public keys
- **Message Operations**: Encrypt/decrypt with public key cryptography
- **Identity Management**: PGP-style identity creation

#### Tab 3: Advanced Settings
- **ECDH Configuration**: Choose cryptographic curves
- **INI Settings**: Save/load configuration files
- **Data Management**: Complete data wipe functionality
- **Logging Controls**: Debug and monitoring options

---

## 2. Password-Based Encryption

This mode uses **ECDH + HKDF + AES-GCM** encryption (NIST 2024 compliant).

### Basic Encryption Workflow

#### Step 1: Set Master Password
```
1. Enter password (8+ characters) → Password field
2. Click "Set Password" button
3. Verify success message appears
4. Password field automatically clears for security
```

#### Step 2: Encrypt a Message
```
1. Type your message → Large message text area
2. (Optional) Configure disappearing messages → See Section 6
3. Click "Encrypt" button
4. Encrypted text appears and copies to clipboard automatically
5. Success notification shows encryption details
```

#### Step 3: Decrypt a Message
```
1. Copy encrypted text to clipboard (or import from file)
2. Enter decryption password → Decryption password field
3. Click "Decrypt" button
4. Decrypted message appears in decryption area
5. Password field clears automatically for security
```

### Supported Features

✅ **Modern Cryptography**: ECDH + HKDF + AES-GCM  
✅ **Disappearing Messages**: Time-based message expiration  
✅ **File Encryption**: Create .enc files  
✅ **Clipboard Integration**: Automatic copy/paste  
✅ **Legacy Support**: Backwards compatibility with older versions  

### Error Handling

| Error Message | Cause | Solution |
|---------------|-------|----------|
| "Enter password first!" | No password set | Set master password first |
| "Password must be at least 8 characters" | Weak password | Use longer, stronger password |
| "Wrong password or data is corrupted" | Incorrect decryption password | Verify password, check data integrity |
| "Message has expired" | Disappearing message expired | Message cannot be recovered |

---

## 3. X25519 Public Key Encryption

This mode uses **X25519 + AES-GCM** encryption with stateless sessions.

### Initial Setup

#### Step 1: Generate X25519 Key Pair
```
1. Go to Tab 2 (X25519 Public Key Encryption)
2. Click "Generate Keys" button (top-left)
3. Your public key appears in the large text area
4. Success message shows key generation details
5. Keys are automatically saved using Windows DPAPI
```

**⚠️ WARNING**: If keys already exist, generation will be blocked to prevent data loss.

#### Step 2: Share Your Public Key
```
1. Double-click public key text area → Copies to clipboard
2. Share via secure channel (email, secure chat, etc.)
3. Recipients need this key to send you encrypted messages
```

#### Step 3: Import Recipient's Public Key
```
1. Click "Recipient" button
2. Paste recipient's public key in dialog box
3. Click "Import"
4. System validates key format and saves recipient info
5. Ready to send encrypted messages
```

### Encryption Workflow

#### Sending Encrypted Messages
```
1. Ensure recipient key is imported
2. Type message → Message input area
3. (Optional) Enable disappearing messages → Toggle switches
4. Click "Encrypt" button
5. Encrypted message appears and copies to clipboard
6. Share encrypted text with recipient
```

#### Receiving Encrypted Messages  
```
1. Copy received encrypted text to clipboard
2. Click "Decrypt" button
3. System automatically detects message type
4. Decrypted message appears in decryption area
5. Sender information displayed (if available)
```

### Advanced Features

#### Identity Management
- **Create Identity**: Generate PGP-style identity with display name
- **Random ID**: 8-character unique identifier
- **Passphrase**: Auto-generated secure passphrase
- **Public Key Export**: ASCII-armored format

#### Contact Trust Levels
- **Unknown** 🟡: First-time contacts (security warning shown)
- **Known** 🟢: Previously contacted (trusted)
- **Trusted** 🔵: Explicitly verified (highest trust)
- **Untrusted** 🔴: Marked as suspicious (warning required)

### UI Components Reference

| Component | Function |
|-----------|----------|
| RSGenerate | Generate new X25519 key pair |
| RSPublickeymsgbox | Display/copy your public key |
| RSWritemsgbox | Input area for messages to encrypt |
| RSDecryptmsgbox | Display area for decrypted messages |
| RsEncrypt | Encrypt message for recipient |
| RSDecrypt | Decrypt received message |
| RecipientBtn | Import recipient's public key |
| CreateNewIDbtn | Create new PGP identity |

---

## 4. Advanced Settings

### ECDH Curve Selection

Configure the elliptic curve used for password-based encryption:

#### Available Options:
1. **Static Key (P-256)** - Default, balanced security/performance
2. **Static Key (P-384)** - Higher security, moderate performance
3. **Static Key (P-521)** - Maximum security, slower performance  
4. **Ephemeral Keys** - New keys per message, maximum forward secrecy

#### Changing ECDH Mode:
```
1. Go to Tab 3 (Advanced Settings)
2. Select desired curve from ECDH dropdown
3. Confirm change in security dialog
4. New mode applies to all future encryptions
5. Previous messages remain compatible
```

⚠️ **WARNING**: Ephemeral mode makes previously encrypted messages unrecoverable.

### INI Configuration Management

#### Save Current Settings:
```
1. Click "Save INI" button
2. Current configuration saved to file
3. Location displayed in confirmation dialog
4. File size and path information provided
```

#### Load Saved Settings:
```
1. Click "Load INI" button  
2. Configuration loaded from existing file
3. If no file exists, option to create defaults
4. Success message shows loaded settings info
```

**Configuration File Location:**  
`%USERPROFILE%\AppData\Roaming\OffCrypt\settings.ini`

### Complete Data Wipe

**⚠️ EXTREME CAUTION**: This permanently deletes ALL OffCrypt data.

#### Data Wipe Process:
```
1. Click "Remove All Data" button
2. Read comprehensive warning dialog carefully
3. Confirm with "YES, DELETE ALL" button
4. System performs secure deletion of:
   - All X25519 key pairs
   - All encrypted storage data
   - Registry entries and configuration
   - Temporary files and cached data
   - Application memory data
5. Application exits automatically
```

**Deleted Items Include:**
- 🔑 All X25519 key pairs (public & private)
- 🔐 All encrypted storage data  
- 📁 All key storage files and folders
- 📋 All registry entries and configuration
- 💾 All temporary files and cached data
- 🗑️ All application memory data

---

## 5. Identity & Contact Management

### Creating PGP Identities

#### Identity Creation Process:
```
1. Go to Tab 2 → Click "Create New ID"
2. Enter optional display name (or leave empty)
3. System generates:
   - Random 8-character ID
   - PGP key pair
   - Secure passphrase
4. Identity appears in public key area
5. Save passphrase securely - it cannot be recovered
```

#### Identity Components:
- **Display Name**: Optional human-readable name
- **Random ID**: Unique 8-character identifier  
- **Key ID**: PGP key identifier
- **Fingerprint**: Cryptographic fingerprint for verification
- **Passphrase**: Auto-generated secure access key

### Contact Trust Management

The system automatically manages contact trust levels based on usage:

#### Trust Level Progression:
1. **First Contact** → Unknown (security warning)
2. **User Confirms** → Known (accepted for communication)  
3. **Manual Upgrade** → Trusted (explicitly verified)
4. **Security Issue** → Untrusted (blocked or flagged)

#### Security Warnings:
- **Unknown Contacts**: First-time encryption shows warning dialog
- **Untrusted Contacts**: Red warning for flagged contacts
- **Fingerprint Display**: Unique identifier for verification

### Contact Data Storage:
- Fingerprints calculated using SHA-256
- Trust levels stored in encrypted configuration
- Message history tracked for usage statistics
- No personal data stored without consent

---

## 6. Disappearing Messages

Disappearing messages automatically expire after a specified time period.

### Password Mode Disappearing Messages

#### Setup Process:
```
1. Go to Tab 1 (Password-Based Encryption)
2. Enable "Disappearing Messages" toggle
3. Select time unit (Hour/Day/Week/Month checkbox)
4. Enter number in text field (e.g., "3" for 3 hours)
5. Type your message
6. Click "Encrypt" - creates time-limited message
```

#### Supported Time Units:
- **Hours**: 1-99 hours
- **Days**: 1-99 days  
- **Weeks**: 1-99 weeks
- **Months**: 1-99 months

### X25519 Mode Disappearing Messages

#### Setup Process:
```
1. Go to Tab 2 (X25519 Public Key Encryption)
2. Enable "Disappearing X255" toggle (bottom section)
3. Select time unit using X255 checkboxes
4. Enter duration in DispX255txtbox
5. Encrypt message normally
6. Recipient sees expiration warning on decrypt
```

#### UI Components:
- **ToggleDispX255**: Enable/disable disappearing messages
- **HourX255Chck**: Hour-based expiration
- **DayX255Chck**: Day-based expiration  
- **WeekX255Chck**: Week-based expiration
- **MonthX255Chck**: Month-based expiration
- **DispX255txtbox**: Duration number input

### Technical Implementation

#### Encryption Process:
1. Message wrapped with JSON metadata containing:
   - Original message content
   - Creation timestamp
   - Expiration timestamp  
   - Message type and version
   - Session and sender information (X25519)

2. Metadata encrypted using selected algorithm
3. Version byte added to identify disappearing message

#### Decryption Process:
1. System detects disappearing message version byte
2. Decrypts and parses JSON metadata
3. Compares current time to expiration time
4. If expired: Shows expiration notice, blocks access
5. If valid: Returns original message content

#### Expiration Handling:
- **Timing-safe comparison**: Prevents timing attacks
- **Permanent deletion**: Expired messages cannot be recovered
- **Clear error messages**: User informed of expiration status
- **Secure cleanup**: Sensitive data cleared from memory

### Security Considerations

✅ **Cryptographically Secure**: Same encryption as regular messages  
✅ **Client-side Expiration**: No server dependency  
✅ **Forward Secrecy**: Expired messages permanently inaccessible  
✅ **Metadata Protection**: Timestamps and settings encrypted  
⚠️ **Clock Dependency**: Requires accurate system time  
⚠️ **No Remote Revocation**: Cannot expire messages on recipient's system  

---

## 7. Security Best Practices

### Password Security

#### Master Password Guidelines:
- **Length**: Minimum 12 characters recommended
- **Complexity**: Mix uppercase, lowercase, numbers, symbols
- **Uniqueness**: Don't reuse passwords from other services
- **Storage**: Use password manager or secure physical storage
- **Changes**: Update periodically, especially after security incidents

#### Password Management:
```
✅ DO:
- Use unique, strong passwords
- Set password immediately on first run
- Clear password from memory when done
- Verify password before important operations

❌ DON'T:  
- Share passwords over insecure channels
- Write passwords in plaintext files
- Use predictable patterns or personal information
- Leave password fields populated when not in use
```

### Key Management

#### X25519 Key Pairs:
- **Generation**: Keys generated using cryptographically secure randomness
- **Storage**: Private keys protected with Windows DPAPI
- **Backup**: Export and securely store public keys
- **Sharing**: Only share public keys, never private keys
- **Validation**: Verify recipient public keys through secure channels

#### Key Lifecycle:
1. **Generation**: Secure random generation with OS entropy
2. **Storage**: DPAPI encryption for private key protection  
3. **Usage**: Memory-safe operations with automatic cleanup
4. **Rotation**: Generate new keys periodically for forward secrecy
5. **Disposal**: Secure deletion when no longer needed

### Communication Security

#### Secure Workflow:
```
1. Verify recipient identity through separate channel
2. Exchange public keys via secure method
3. Verify key fingerprints manually  
4. Use disappearing messages for sensitive content
5. Regularly update trust levels based on interactions
```

#### Trust Verification:
- **Fingerprint Comparison**: Manually verify key fingerprints
- **Out-of-band Verification**: Confirm keys via phone, in-person, etc.
- **Trust Levels**: Use contact trust system appropriately
- **Regular Reviews**: Periodically review trusted contacts

### Application Security

#### Memory Management:
- Passwords stored as char[] arrays, not strings
- Automatic memory clearing after operations
- Secure disposal of sensitive data structures
- Garbage collection forced after operations

#### Data Protection:
- **Windows DPAPI**: Private keys encrypted with user profile
- **Registry Cleanup**: Settings can be completely removed
- **File Cleanup**: Temporary files securely deleted
- **Clipboard Management**: Automatic clearing after operations

### Operational Security

#### Environment:
- Use on trusted, malware-free systems
- Keep OS and antivirus updated  
- Avoid shared or public computers
- Use full-disk encryption on storage devices

#### Backup and Recovery:
- Export public keys for backup
- Securely store PGP passphrases
- Document trusted contact fingerprints
- Test recovery procedures regularly

---

## 8. File Operations

### Encrypted File Creation

#### Password-Based File Encryption:
```
1. Enter message in Tab 1 message area
2. Configure disappearing messages (optional)
3. Click "Create Encrypted File" button
4. Choose save location in file dialog
5. File saved with .enc extension
6. Confirmation shows file details and location
```

#### X25519 File Encryption:
```
1. Encrypt message normally in Tab 2
2. Click "Create File" button  
3. Choose save location in file dialog
4. File saved with .pgp extension
5. File contains encrypted message ready for sharing
```

### File Import and Decryption

#### Importing Encrypted Files:
```
1. Click "Import" button (Tab 1 for .enc, Tab 2 for .pgp)
2. Select encrypted file in file dialog
3. File content copied to clipboard automatically  
4. Ready for decryption using normal process
5. Success message confirms file loaded
```

#### Supported File Formats:
- **.enc**: Password-based encrypted files
- **.pgp**: X25519/PGP encrypted files
- **.***: Any text file containing encrypted content

### File Management Best Practices

#### File Naming:
- **Descriptive Names**: Include date, recipient, purpose
- **No Sensitive Info**: Avoid revealing content in filename
- **Consistent Format**: Use standard naming conventions
- **Version Control**: Include version numbers for updates

#### Storage Recommendations:
```
✅ SECURE STORAGE:
- Encrypted external drives
- Cloud storage with client-side encryption
- Password-protected archives
- Air-gapped backup systems

❌ AVOID:
- Unencrypted cloud storage
- Shared network drives
- Temporary folders
- Email attachments for long-term storage
```

#### File Operations Security:
- **Automatic Cleanup**: Temporary files securely deleted
- **Memory Management**: File contents cleared after processing  
- **Path Validation**: File paths checked for security issues
- **Error Handling**: Graceful handling of corrupted files

---

## 9. Troubleshooting

### Common Issues and Solutions

#### Password Problems

**Issue**: "Password must be at least 8 characters long"
```
Solution:
1. Use longer password (minimum 8 characters)
2. Include mix of character types
3. Avoid common dictionary words
4. Test password strength before setting
```

**Issue**: "Wrong password or data is corrupted"  
```
Troubleshooting Steps:
1. Verify password spelling and case
2. Check for extra spaces or hidden characters
3. Try copying password from password manager
4. Verify data integrity of encrypted text
5. Check if message has expired (disappearing messages)
```

#### Key Management Issues

**Issue**: "X25519 key pair already exists"
```
Solution:
1. This is a security feature to prevent key loss
2. Use existing keys or create new identity
3. For new keys: Go to Identity Manager → Create New Identity
4. Export existing keys before generating new ones
```

**Issue**: "Failed to import recipient key"
```
Troubleshooting:
1. Verify key format (should be ASCII-armored)
2. Check for complete key (including BEGIN/END lines)
3. Remove extra whitespace or formatting
4. Verify key is actually a public key, not private
5. Try copying key directly from source
```

#### Encryption/Decryption Errors

**Issue**: Message fails to encrypt
```
Diagnostic Steps:
1. Check if master password is set (Tab 1)  
2. Verify recipient key imported (Tab 2)
3. Check message length (very long messages may timeout)
4. Disable disappearing messages temporarily
5. Check available system memory
```

**Issue**: "Message has expired and was destroyed"
```
Understanding:
- This is normal behavior for disappearing messages
- Message cannot be recovered once expired
- Check creation and expiration timestamps
- Sender must create new message if needed
```

#### UI and Performance Issues

**Issue**: Application runs slowly
```
Optimization Steps:  
1. Close unnecessary applications
2. Increase available RAM
3. Check for Windows updates
4. Restart application periodically
5. Clear clipboard history
6. Run disk cleanup utilities
```

**Issue**: UI elements not responding
```
Recovery Actions:
1. Check if operation is in progress (wait for completion)
2. Verify adequate system resources
3. Restart application if frozen
4. Check Windows Event Log for errors
5. Run application as administrator if needed
```

### Error Messages Reference

| Error Code | Message | Cause | Solution |
|------------|---------|-------|----------|
| PWD001 | Enter password first! | No master password set | Set password in Tab 1 |
| PWD002 | Password must be at least 8 characters | Weak password | Use stronger password |
| DEC001 | Wrong password or data corrupted | Incorrect password or bad data | Verify password and data |
| EXP001 | Message has expired | Disappearing message expired | Cannot recover, request new message |
| KEY001 | Keys missing | No X25519 keys generated | Generate keys using RSGenerate |
| KEY002 | Recipient key not found | No recipient imported | Import recipient's public key |
| FILE001 | File import failed | Corrupted or invalid file | Check file integrity and format |
| MEM001 | Insufficient memory | Low system resources | Close other applications, restart |

### Recovery Procedures

#### Complete Application Reset:
```
1. Export important keys and data
2. Close OffCryptDesktop completely
3. Go to Tab 3 → Click "Remove All Data"
4. Restart application (fresh installation state)
5. Reconfigure settings and import keys
6. Test functionality with simple operations
```

#### Partial Data Recovery:
```
1. Check INI configuration files in %APPDATA%\OffCrypt\
2. Look for key backup files in user profile
3. Check Windows Registry: HKEY_CURRENT_USER\SOFTWARE\OffCrypt
4. Review clipboard history for recently copied keys
5. Contact recipients for public key re-sharing
```

#### Emergency Procedures:
```
If application becomes unstable:
1. Save any displayed keys or messages immediately
2. Close application through Task Manager if needed  
3. Check Windows Event Log for crash details
4. Backup any recoverable data before restart
5. Consider running Windows Memory Diagnostic
```

---

## 10. Technical Reference

### Cryptographic Specifications

#### Password-Based Encryption (Tab 1)
**Algorithm**: ECDH + HKDF + AES-GCM
- **Key Exchange**: Elliptic Curve Diffie-Hellman
- **Key Derivation**: HMAC-based Key Derivation Function
- **Encryption**: AES-256 in Galois/Counter Mode
- **Authentication**: Built-in GCM authentication
- **Standards**: NIST SP 800-56A Rev. 3, RFC 5869

#### X25519 Public Key Encryption (Tab 2)  
**Algorithm**: X25519 + AES-GCM
- **Key Exchange**: Curve25519 (RFC 7748)
- **Encryption**: AES-256-GCM
- **Session Management**: Stateless sessions with sequence numbers
- **Message Authentication**: GCM built-in authentication
- **Forward Secrecy**: Session-based key derivation

### Version Compatibility Matrix

| Version Byte | Encryption Type | Compatibility | Status |
|--------------|----------------|---------------|---------|
| 0x01 | Legacy Password (PBKDF2+AES-CBC) | Read-only | Deprecated |
| 0x02 | Legacy Disappearing Password | Full | Legacy |
| 0x03 | Enhanced Legacy (ECDH+PBKDF2) | Full | Legacy |
| 0x10 | Modern Password (ECDH+HKDF+AES-GCM) | Full | Current |
| 0x12 | Modern Disappearing Password | Full | Current |
| 0x0A | X25519 Basic | Full | Current |
| 0x0B | X25519 Settings | Full | Current |
| 0x1B | X25519 Disappearing Modern | Full | Current |

### Platform Requirements

#### Windows Compatibility:
- **OS**: Windows 10 version 1809 or later
- **Framework**: .NET 6.0 or later
- **Architecture**: x64, ARM64
- **Dependencies**: Windows DPAPI, Windows Cryptography API

#### Hardware Requirements:
- **RAM**: 512 MB minimum, 1 GB recommended  
- **Storage**: 100 MB available space
- **Processor**: Any modern x64 or ARM64 CPU
- **Graphics**: DirectX 11 compatible (for UI rendering)

### API Integration Possibilities

#### Command Line Interface:
Currently not available, but application architecture supports:
- Batch processing through file operations  
- Automation via clipboard monitoring
- Integration with PowerShell scripts
- Custom workflow development

#### Programmatic Access:
Core cryptographic classes can be accessed:
- `PasswordSecure` class for password-based operations
- `X25519Manager` class for public key operations  
- `DisappearingMessages` class for time-limited messages
- `ContactManager` class for trust management

### Security Audit Information

#### Code Security Features:
- **Memory Safety**: Char arrays instead of strings for passwords
- **Automatic Cleanup**: Sensitive data cleared after use
- **Exception Handling**: Comprehensive error handling prevents information leaks
- **Input Validation**: All user inputs validated and sanitized
- **Secure Random**: OS-provided cryptographically secure random number generation

#### Security Standards Compliance:
- **NIST**: Follows NIST SP 800-56A for key establishment
- **RFC**: Implements RFC 5869 (HKDF) and RFC 7748 (X25519)
- **FIPS**: Uses FIPS 140-2 validated cryptographic modules where available
- **Common Criteria**: Follows CC protection profiles for cryptographic software

### Advanced Configuration

#### Registry Settings Location:
```
HKEY_CURRENT_USER\SOFTWARE\OffCrypt\
├── ECDHMode (String) - Current ECDH curve selection
├── LastUsed (DateTime) - Last application usage
└── Version (String) - Application version information
```

#### Configuration File Format (INI):
```
[General]
Theme=Dark
Language=en-US
LogLevel=Info

[Encryption]  
ECDHMode=StaticP256
DefaultTimeUnit=Hour
DefaultDuration=3

[Security]
MemoryCleanup=True
ClipboardTimeout=30
AutoLock=False
```

#### Logging Configuration:
- **Location**: `%TEMP%\OffCrypt\Logs\`
- **Levels**: Debug, Info, Warning, Error, Critical
- **Rotation**: Daily log files with 30-day retention
- **Privacy**: No sensitive data logged (keys, passwords, message content)

---

## Support and Updates

### Getting Help
- **Documentation**: This manual covers all standard operations
- **Issue Reporting**: Check application logs for error details
- **Community**: Share experiences with other users (without revealing sensitive information)

### Best Practices Summary
1. **Strong Passwords**: Use unique, complex passwords
2. **Key Verification**: Always verify recipient keys through secure channels  
3. **Regular Updates**: Keep application and OS updated
4. **Backup Strategy**: Securely backup keys and important encrypted data
5. **Trust Management**: Properly configure and maintain contact trust levels
6. **Secure Environment**: Use on trusted, well-maintained systems

### Security Notices
- **Zero Trust**: Never assume any communication channel is completely secure
- **Forward Secrecy**: Use ephemeral modes and disappearing messages for maximum security
- **Key Rotation**: Regularly generate new keys and update recipients
- **Incident Response**: Have procedures for compromised keys or passwords

---

**End of Manual**

*This manual covers OffCryptDesktop Version 2.0. For the latest updates and security notices, check the application's built-in help system and logging features.*