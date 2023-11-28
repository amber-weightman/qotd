import FileCopyIcon from '@mui/icons-material/FileCopyOutlined';
import RefreshIcon from '@mui/icons-material/Refresh';
import { Alert, Box, Snackbar, SpeedDial, SpeedDialAction, SpeedDialIcon } from "@mui/material";
import { useState } from "react";

interface AppMenuProps {
  question: string | undefined,
  lookupQuestion: () => Promise<void>
}

function AppMenu(props: AppMenuProps) {
  const [openToast, setOpenToast] = useState(false);
  const [toastMessage, setToastMessage] = useState<string | undefined>();

  const enum menuOptions {
    'COPY',
    'REFRESH'
  }

  const actions = [
    { icon: <FileCopyIcon />, name: 'Copy', operation: menuOptions.COPY },
    { icon: <RefreshIcon />, name: 'Refresh', operation: menuOptions.REFRESH }
  ];

  const handleClick = (operation: menuOptions) => {
    if (operation === menuOptions.REFRESH) {
      props.lookupQuestion();
    } else if (operation === menuOptions.COPY && props.question) {
      navigator.clipboard.writeText(props.question);
      setToastMessage('Copied to clipboard');
      setOpenToast(true);
    }
  };

  const handleClose = () => {
    setToastMessage(undefined);
    setOpenToast(false);
  };

  return (
    <Box sx={{ height: 320, flexGrow: 1 }}>
      <SpeedDial
        ariaLabel="SpeedDial basic example"
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
      <Snackbar
        open={openToast}
        autoHideDuration={3000}
        onClose={handleClose}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
      >
        <Alert
          onClose={handleClose}
          severity="success"
          sx={{ width: '100%' }}
        >
          {toastMessage}
        </Alert>
      </Snackbar>
    </Box>
  );
}

export default AppMenu;