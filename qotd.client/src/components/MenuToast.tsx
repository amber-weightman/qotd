import { Alert, AlertColor, Snackbar } from "@mui/material";

interface MenuToastProps {
  open: boolean,
  message: string | undefined,
  severity: AlertColor | undefined,
  handleClose: () => void,
  autoHideDuration: number | null | undefined,
  vertical: 'top' | 'bottom',
  horizontal: 'left' | 'center' | 'right'
}

MenuToast.defaultProps = {
  autoHideDuration: 3000,
  vertical: 'bottom',
  horizontal: 'right'
};

function MenuToast(props: MenuToastProps) {

  return (
    <Snackbar
      open={props.open}
      autoHideDuration={props.autoHideDuration}
      onClose={props.handleClose}
      anchorOrigin={{ vertical: props.vertical, horizontal: props.horizontal }}
    >
      <Alert
        onClose={props.handleClose}
        severity={props.severity}
        sx={{ width: '100%' }}
      >
        {props.message}
      </Alert>
    </Snackbar>
  );
}

export default MenuToast;