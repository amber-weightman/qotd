import FileCopyIcon from '@mui/icons-material/FileCopyOutlined';
import RefreshIcon from '@mui/icons-material/Refresh';
import SettingsIcon from '@mui/icons-material/Settings';
import InfoIcon from '@mui/icons-material/Info';
import { AlertColor, Box, SpeedDial, SpeedDialAction, SpeedDialIcon } from "@mui/material";
import { useState } from "react";
import MenuToast from './MenuToast';
import AppDrawer from './AppDrawer';
import About from './About';

interface AppMenuProps {
  question: string | undefined,
  lookupQuestion: () => Promise<void>
}

function AppMenu(props: AppMenuProps) {
  const [openToast, setOpenToast] = useState(false);
  const [toastMessage, setToastMessage] = useState<string | undefined>();
  const [toastSeverity, setToastSeverity] = useState<AlertColor | undefined>();

  const [openDrawer, setOpenDrawer] = useState(false);
  const [openAbout, setOpenAbout] = useState(false);

  const enum menuOptions {
    'COPY',
    'REFRESH',
    'SETTINGS',
    'ABOUT'
  }

  const actions = [
    { icon: <FileCopyIcon />, name: 'Copy', operation: menuOptions.COPY },
    { icon: <RefreshIcon />, name: 'Reload', operation: menuOptions.REFRESH },
    { icon: <SettingsIcon />, name: 'Settings', operation: menuOptions.SETTINGS },
    { icon: <InfoIcon />, name: 'About', operation: menuOptions.ABOUT }
  ];

  const handleClick = (operation: menuOptions) => {
    switch (operation) {
      case menuOptions.REFRESH:
        props.lookupQuestion();
        break;
      case menuOptions.SETTINGS:
        handleOpenSettings();
        break;
      case menuOptions.ABOUT:
        handleOpenAbout();
        break;
      case menuOptions.COPY:
        handleCopy();
        break;
    }
  };

  const handleOpenToast = (message: string, severity: AlertColor) => {
    setToastMessage(message);
    setToastSeverity(severity);
    setOpenToast(true);
  };

  const handleCloseToast = () => {
    setToastMessage(undefined);
    setOpenToast(false);
    setToastSeverity(undefined);
  };

  const handleCopy = () => {
    if (props.question) {
      navigator.clipboard.writeText(props.question);
      handleOpenToast('Copied to clipboard', 'success');
    } else {
      handleOpenToast('Something went wrong', 'error');
    }
  };

  const handleOpenSettings = () => {
    handleOpenToast('Not implemented yet', 'warning');
    setOpenDrawer(true);
  };

  const handleOpenAbout = () => {
    setOpenDrawer(true);
    setOpenAbout(true);
  };

  const handleDrawerClose = () => {
    setOpenDrawer(false);
    setOpenAbout(false);
  };

  return (
    <Box sx={{ height: 320, flexGrow: 1 }}>
      <SpeedDial
        ariaLabel="Website menu"
        sx={{ position: 'absolute', bottom: 32, right: 32 }}
        icon={<SpeedDialIcon />}
      >
        {actions.map((action) => (
          <SpeedDialAction
            key={action.name}
            icon={action.icon}
            tooltipTitle={action.name}
            onClick={() => {
              handleClick(action.operation)
            }}
          />
        ))}
      </SpeedDial>
      <MenuToast
        open={openToast}
        message={toastMessage}
        severity={toastSeverity}
        handleClose={handleCloseToast}
      />
      <AppDrawer open={openDrawer} handleClose={handleDrawerClose}>
        {openAbout && <About />}
      </AppDrawer>
    </Box>
  );
}

export default AppMenu;