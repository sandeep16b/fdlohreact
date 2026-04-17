import { SearchReceivableReport } from "./components/SearchReceivableReport";
import { CreateReceivableReport } from "./components/CreateReceivableReport";
import { ViewReceivableReport } from "./components/ViewReceivableReport";
import { SearchApplicationSecurity } from "./components/SearchApplicationSecurity";
import { CreateApplicationSecurity } from "./components/CreateApplicationSecurity";

const AppRoutes = [
  {
    path: '/',
    element: <SearchReceivableReport />
  },
  {
    path: '/receivable-report/search',
    element: <SearchReceivableReport />
  },
  {
    path: '/receivable-report/create',
    element: <CreateReceivableReport />
  },
  {
    path: '/receivable-report/edit/:id',
    element: <CreateReceivableReport />
  },
  {
    path: '/receivable-report/view/:id',
    element: <ViewReceivableReport />
  },
  {
    path: '/application-security/search',
    element: <SearchApplicationSecurity />
  },
  {
    path: '/application-security/create',
    element: <CreateApplicationSecurity />
  },
  {
    path: '/application-security/edit/:id',
    element: <CreateApplicationSecurity />
  }
];

export default AppRoutes;
