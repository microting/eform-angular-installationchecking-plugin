import { Injectable } from '@angular/core';
import { persistState, Store, StoreConfig } from '@datorama/akita';
import {
  CommonPaginationState,
  FiltrationStateModel,
} from 'src/app/common/models';
import { InstallationStateEnum, InstallationTypeEnum } from '../../../const';

export interface RemovalState {
  pagination: CommonPaginationState;
  filters: InstallationFiltrationState;
  total: number;
}

export class InstallationFiltrationState extends FiltrationStateModel {
  type: InstallationTypeEnum;
  state: InstallationStateEnum;
}

function createInitialState(): RemovalState {
  return <RemovalState>{
    pagination: {
      pageSize: 10,
      sort: 'Id',
      isSortDsc: false,
      offset: 0,
    },
    filters: {
      nameFilter: '',
      type: InstallationTypeEnum.Removal,
      state: 0,
    },
    total: 0,
  };
}

const removalPersistStorage = persistState({
  include: ['removal'],
  key: 'installationCheckingPn',
  preStorageUpdate(storeName, state) {
    return {
      pagination: state.pagination,
      filters: state.filters,
    };
  },
});

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'removal', resettable: true })
export class RemovalStore extends Store<RemovalState> {
  constructor() {
    super(createInitialState());
  }
}

export const removalPersistProvider = {
  provide: 'persistStorage',
  useValue: removalPersistStorage,
  multi: true,
};
