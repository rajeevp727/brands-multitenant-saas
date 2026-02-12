import React, { useState, useEffect, useRef } from "react";
import {
    Badge,
    IconButton,
    Box,
    Typography,
    List,
    ListItem,
    ListItemText,
    Paper,
    ClickAwayListener,
    Divider,
    ListItemButton,
    Tooltip
} from "@mui/material";
import NotificationsIcon from "@mui/icons-material/Notifications";
import CircleIcon from "@mui/icons-material/Circle";
import { notificationService } from "../../services/notificationService";
import type { NotificationItem } from "../../types/notification";

const NotificationBell: React.FC = () => {
    const [open, setOpen] = useState(false);
    const [notifications, setNotifications] = useState<NotificationItem[]>([]);
    const [unreadCount, setUnreadCount] = useState(0);
    const anchorRef = useRef<HTMLButtonElement>(null);

    const isAuthenticated = !!localStorage.getItem("user");

    const fetchNotifications = React.useCallback(async () => {
        if (!isAuthenticated) return;

        try {
            const [data, count] = await Promise.all([
                notificationService.getNotifications(),
                notificationService.getUnreadCount()
            ]);
            setNotifications(data);
            setUnreadCount(count);
        } catch (error) {
            console.error("Failed to fetch notifications", error);
        }
    }, [isAuthenticated]);

    useEffect(() => {
        if (!isAuthenticated) return;

        fetchNotifications().catch(console.error);
        const interval = setInterval(() => { fetchNotifications().catch(console.error); }, 60000);
        return () => clearInterval(interval);
    }, [isAuthenticated, fetchNotifications]);

    const handleToggle = () => {
        if (!isAuthenticated) return;

        setOpen(prev => !prev);
        if (!open) {
            fetchNotifications();
        }
    };

    const handleClose = (event: Event | React.SyntheticEvent) => {
        if (anchorRef.current && anchorRef.current.contains(event.target as HTMLElement)) {
            return;
        }
        setOpen(false);
    };

    const handleMarkAsRead = async (item: NotificationItem) => {
        if (!item.isRead) {
            try {
                await notificationService.markAsRead(item.id);
                setNotifications(prev =>
                    prev.map(n => (n.id === item.id ? { ...n, isRead: true } : n))
                );
                setUnreadCount(prev => Math.max(0, prev - 1));
            } catch (error) {
                console.error("Failed to mark as read", error);
            }
        }
    };

    // Don't render bell at all if not logged in
    if (!isAuthenticated) return null;

    return (
        <>
            <Tooltip title="Notifications">
                <IconButton
                    color="inherit"
                    ref={anchorRef}
                    onClick={handleToggle}
                    sx={{ mr: 2 }}
                >
                    <Badge badgeContent={unreadCount} color="error">
                        <NotificationsIcon />
                    </Badge>
                </IconButton>
            </Tooltip>

            {open && (
                <ClickAwayListener onClickAway={handleClose}>
                    <Paper
                        sx={{
                            position: "absolute",
                            top: 60,
                            right: 20,
                            width: 360,
                            maxHeight: 500,
                            overflow: "auto",
                            zIndex: 1300,
                            boxShadow: 3
                        }}
                    >
                        <Box
                            sx={{
                                p: 2,
                                display: "flex",
                                alignItems: "center",
                                justifyContent: "space-between"
                            }}
                        >
                            <Typography variant="h6">Notifications</Typography>
                            {unreadCount > 0 && (
                                <Typography variant="caption" color="error">
                                    {unreadCount} New
                                </Typography>
                            )}
                        </Box>

                        <Divider />

                        <List sx={{ p: 0 }}>
                            {notifications.length === 0 ? (
                                <ListItem>
                                    <ListItemText
                                        primary="No notifications"
                                        secondary="You're all caught up!"
                                    />
                                </ListItem>
                            ) : (
                                notifications.map(notification => (
                                    <React.Fragment key={notification.id}>
                                        <ListItemButton
                                            onClick={() => handleMarkAsRead(notification)}
                                            alignItems="flex-start"
                                            sx={{
                                                backgroundColor: notification.isRead
                                                    ? "inherit"
                                                    : "rgba(25, 118, 210, 0.04)",
                                                "&:hover": { backgroundColor: "rgba(0, 0, 0, 0.04)" }
                                            }}
                                        >
                                            <ListItemText
                                                primary={
                                                    <Box
                                                        component="span"
                                                        sx={{ display: "flex", alignItems: "center", gap: 1 }}
                                                    >
                                                        {!notification.isRead && (
                                                            <CircleIcon
                                                                sx={{ fontSize: 10, color: "primary.main" }}
                                                            />
                                                        )}
                                                        <Typography
                                                            variant="subtitle2"
                                                            component="span"
                                                            fontWeight={
                                                                notification.isRead ? "normal" : "bold"
                                                            }
                                                        >
                                                            {notification.title}
                                                        </Typography>
                                                    </Box>
                                                }
                                                secondary={
                                                    <>
                                                        <Typography
                                                            sx={{ display: "block", mt: 0.5 }}
                                                            component="span"
                                                            variant="body2"
                                                            color="text.primary"
                                                        >
                                                            {notification.message}
                                                        </Typography>
                                                        <Typography
                                                            variant="caption"
                                                            color="text.secondary"
                                                            sx={{ mt: 0.5, display: "block" }}
                                                        >
                                                            {notification.timeAgo} • {notification.type}
                                                        </Typography>
                                                    </>
                                                }
                                            />
                                        </ListItemButton>
                                        <Divider component="li" />
                                    </React.Fragment>
                                ))
                            )}
                        </List>
                    </Paper>
                </ClickAwayListener>
            )}
        </>
    );
};

export default NotificationBell;
