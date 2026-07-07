variable "resource_group_name" {
  description = "Resource group that holds the AionWorkerNode VM and its networking."
  type        = string
}

variable "location" {
  description = "Azure region, e.g. 'westeurope', 'francecentral'."
  type        = string
}

variable "vm_name" {
  description = "Name of the VM (matches virtualMachines_AionWorkerNode_name in the ARM parameters)."
  type        = string
  default     = "AionWorkerNode"
}

variable "admin_username" {
  description = "Linux admin username for the VM."
  type        = string
  default     = "azureuser"
}

variable "ssh_public_key" {
  description = "SSH public key content (matches sshPublicKeys_AionWorkerNode_key_name)."
  type        = string
  sensitive   = true
}

variable "vm_size" {
  description = "Azure VM size / SKU."
  type        = string
  default     = "Standard_B2s"
}

variable "os_disk_type" {
  description = "OS disk storage account type."
  type        = string
  default     = "StandardSSD_LRS"
}

variable "image_publisher" {
  type    = string
  default = "Canonical"
}

variable "image_offer" {
  type    = string
  default = "0001-com-ubuntu-server-jammy"
}

variable "image_sku" {
  type    = string
  default = "22_04-lts-gen2"
}

variable "image_version" {
  type    = string
  default = "latest"
}

variable "vnet_name" {
  description = "Matches virtualNetworks_AionWorkerNode_vnet_name."
  type        = string
  default     = "AionWorkerNode-vnet"
}

variable "vnet_address_space" {
  type    = string
  default = "10.20.0.0/16"
}

variable "subnet_name" {
  type    = string
  default = "default"
}

variable "subnet_address_prefix" {
  type    = string
  default = "10.20.1.0/24"
}

variable "nsg_name" {
  description = "Matches networkSecurityGroups_AionWorkerNode_nsg_name."
  type        = string
  default     = "AionWorkerNode-nsg"
}

variable "nic_name" {
  description = "Matches networkInterfaces_aionworkernode1_name."
  type        = string
  default     = "aionworkernode1"
}

variable "allowed_ssh_source_cidr" {
  description = "CIDR (or 'Internet' for any) allowed to reach SSH on port 22. Since this box also joins your tailnet via Tailscale, you may want to lock this down to your office/VPN IP rather than leaving it open."
  type        = string
  default     = "*"
}

variable "assign_public_ip" {
  description = "Whether to attach a public IP. If the VM only needs to be reachable over Tailscale, set this to false."
  type        = bool
  default     = true
}

variable "shutdown_schedule_name" {
  description = "Matches schedules_shutdown_computevm_aionworkernode_name."
  type        = string
  default     = "shutdown-computevm-aionworkernode"
}

variable "shutdown_time" {
  description = "24h HHmm time to auto shut down the VM daily."
  type        = string
  default     = "1900"
}

variable "shutdown_timezone" {
  type    = string
  default = "Central European Standard Time"
}

variable "shutdown_notification_enabled" {
  type    = bool
  default = false
}

variable "notification_email" {
  description = "Email for shutdown notifications (only used if shutdown_notification_enabled = true)."
  type        = string
  default     = ""
}

variable "cloud_init" {
  description = "Optional cloud-init script run on first boot (e.g. to install Tailscale and join your tailnet)."
  type        = string
  default     = <<-EOT
    #cloud-config
    package_update: true
    runcmd:
      - curl -fsSL https://tailscale.com/install.sh | sh
      - tailscale up --ssh
  EOT
}

variable "tags" {
  type    = map(string)
  default = {
    project = "SilkRoad"
    role    = "AionWorkerNode"
    managed = "terraform"
  }
}
