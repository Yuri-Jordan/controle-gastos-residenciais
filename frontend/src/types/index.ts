// Tipos relacionados a Pessoa
export interface Pessoa {
    id: string;
    nome: string;
    idade: number;
    ehMenorDeIdade: boolean;
}

export interface CriarPessoa {
    nome: string;
    idade: number;
}

export interface AtualizarPessoa {
    nome: string;
    idade: number;
}

// Tipos relacionados a Categoria
export enum FinalidadeCategoria {
    Despesa = 0,
    Receita = 1,
    Ambos = 2
}

export interface Categoria {
    id: string;
    descricao: string;
    finalidade: FinalidadeCategoria;
    exibirFinalidade: string;
}

export interface CriarCategoria {
    descricao: string;
    finalidade: FinalidadeCategoria;
}

// Tipos relacionados a Transação
export enum TipoTransacao {
    Despesa = 0,
    Receita = 1
}

export interface Transacao {
    id: string;
    descricao: string;
    valor: number;
    tipo: TipoTransacao;
    exibirTipo: string;
    categoriaId: string;
    descricaoCategoria: string;
    pessoaId: string;
    nomePessoa: string;
    criadoEm: string;
}

export interface CriarTransacao {
    descricao: string;
    valor: number;
    tipo: TipoTransacao;
    categoriaId: string;
    pessoaId: string;
}

// Tipos de Relatórios
export interface TotaisPessoa {
    pessoaId: string;
    nomePessoa: string;
    totalReceita: number;
    totalDespesa: number;
    saldo: number;
}

export interface TotaisCategoria {
    categoriaId: string;
    descricaoCategoria: string;
    finalidadeCategoria: string;
    totalReceita: number;
    totalDespesa: number;
    saldo: number;
}

export interface TotaisResumo {
    receitaTotal: number;
    despesaTotal: number;
    saldoLiquido: number;
}

export interface RelatorioTotaisPessoa {
    totaisPessoa: TotaisPessoa[];
    resumo: TotaisResumo;
}

export interface RelatorioTotaisCategoria {
    totaisCategoria: TotaisCategoria[];
    resumo: TotaisResumo;
}

// Tipos de Resposta da API
export interface RespostaApi<T> {
    dados: T;
    mensagem?: string;
    sucesso: boolean;
}