variable "location" {
  description = "Azure region for all resources"
  type        = string
  default     = "eastus"
}

variable "environment" {
  description = "Environment (dev or prod)"
  type        = string
  default     = "dev"

  validation {
    condition     = contains(["dev", "prod"], var.environment)
    error_message = "Environment must be dev or prod."
  }
}

variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
}

variable "hub_address_space" {
  description = "Hub VNet address prefix"
  type        = string
  default     = "10.0.0.0/16"
}

variable "compute_spoke_address_space" {
  description = "Compute spoke VNet address prefix"
  type        = string
  default     = "10.1.0.0/16"
}

variable "data_spoke_address_space" {
  description = "Data spoke VNet address prefix"
  type        = string
  default     = "10.2.0.0/16"
}

variable "aks_node_count" {
  description = "Number of AKS agent nodes in user pool"
  type        = number
  default     = 2
}

variable "aks_vm_size" {
  description = "AKS agent node VM size"
  type        = string
  default     = "Standard_D2s_v3"
}

variable "aks_kubernetes_version" {
  description = "AKS Kubernetes version"
  type        = string
  default     = "1.28"
}

variable "sql_administrator_login" {
  description = "SQL administrator login"
  type        = string
}

variable "sql_administrator_password" {
  description = "SQL administrator password"
  type        = string
  sensitive   = true
}

variable "enable_azure_firewall" {
  description = "Enable Azure Firewall in Hub"
  type        = bool
  default     = true
}

variable "enable_private_cluster" {
  description = "Enable private AKS cluster"
  type        = bool
  default     = false
}

variable "project_name" {
  description = "Project name for resource naming"
  type        = string
  default     = "silkroad"
}
