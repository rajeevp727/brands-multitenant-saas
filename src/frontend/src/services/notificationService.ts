import api from "./api";
import type { NotificationItem } from "../types/notification";

export const notificationService = {
    async getNotifications() {
        const res = await api.get<NotificationItem[]>("/notifications");
        return res.data;
    },

    async getUnreadCount() {
        const res = await api.get<number>("/notifications/unread-count");
        return res.data;
    },

    async markAsRead(id: string) {
        await api.put(`/notifications/${id}/read`);
    }
};
