export interface NotificationItem {
    id: string;
    title: string;
    message: string;
    type: string;
    severity: string;
    isRead: boolean;
    createdAt: string;
    timeAgo: string;
}
